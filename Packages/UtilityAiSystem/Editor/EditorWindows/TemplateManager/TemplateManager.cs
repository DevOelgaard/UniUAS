using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using UnityEditor.UIElements;

internal class TemplateManager : EditorWindow
{
    private IDisposable activeCollectionChangedSub;

    private CompositeDisposable modelsChangedSubsciptions = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftPanel;
    private VisualElement elementsContainer;
    private VisualElement rightPanel;
    private VisualElement buttonContainer;
    private Button createNewButton;
    private Button copyButton;
    private Button deleteButton;
    private Button resetButton;
    private Button saveButton;
    private Button loadButton;
    private Button refreshButton;
    private Button exportButton;
    private Button importButton;
    private Button saveToPlayButton;
    private Button restoreButton;
    private PopupField<string> addElementPopup;
    private UASTemplateService uASTemplateService => UASTemplateService.Instance;

    private MainWindowComponent mainWindowComponent;
    private AiObjectModel selectedModel;
    private PersistenceAPI persistenceAPI = new PersistenceAPI(new JSONPersister());
    private AiObjectModel SelectedModel
    {
        get => selectedModel;
        set
        {
            selectedModel = value;
            UpdateButtons();
        }
    }

    private List<string> dropDownChoices;
    private DropdownField dropDown;

    [MenuItem(Consts.MenuName + Consts.Name_TemplateManager)]
    internal static void ShowExample()
    {
        TemplateManager wnd = GetWindow<TemplateManager>();
        wnd.titleContent = new GUIContent(Consts.Name_TemplateManager);
        wnd.Show();
        wnd.position = new Rect(0f, 0f, 1920/3, 1080/2);
    }

    internal void CreateGUI()
    {
        var state = persistenceAPI.LoadObjectPath<UASTemplateServiceState>(Consts.File_UASTemplateServicelAutoSave);
        if (state != null)
        {
            uASTemplateService.Restore(state);
        }

        root = rootVisualElement;
        
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);
        dropDown = root.Q<DropdownField>("TypeDropdown");

        leftPanel = root.Q<VisualElement>("left-panel");
        elementsContainer = leftPanel.Q<VisualElement>("ButtonContainer");

        buttonContainer = root.Q<VisualElement>("Buttons");
        addElementPopup = new PopupField<string>("Add element");
        addElementPopup.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            AddNewAiObject(evt.newValue);
        });

        buttonContainer.Add(addElementPopup);


        rightPanel = root.Q<VisualElement>("right-panel");

        createNewButton = root.Q<Button>("CreateNewButton");
        createNewButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            CreateNewModel();
        });

        copyButton = root.Q<Button>("CopyButton");
        copyButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            CopySelectedModel();
        });

        deleteButton = root.Q<Button>("DeleteButton");
        deleteButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            DeleteSelectedElement();
        });

        resetButton = root.Q<Button>("ResetButton");
        resetButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uASTemplateService.Reset();
            UpdateLeftPanel();
            rightPanel.Clear();
        });

        saveButton = root.Q<Button>("SaveButton");
        saveButton.text = "Save project";
        saveButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            persistenceAPI.SaveObjectPanel(uASTemplateService);
        });

        loadButton = root.Q<Button>("LoadButton");
        loadButton.text = "Load";
        loadButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            var uasState = persistenceAPI.LoadObjectPanel<UASTemplateServiceState>();
            uASTemplateService.Restore(uasState);
            UpdateLeftPanel();
        });

        refreshButton = root.Q<Button>("RefreshButton");
        refreshButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uASTemplateService.Refresh();
            UpdateLeftPanel();
        });

        exportButton = root.Q<Button>("ExportButton");
        exportButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            var saveObjects = new List<RestoreAble>();
            selectedObjects.ForEach(pair => saveObjects.Add(pair.Key));
            var type = MainWindowService.GetTypeFromString(dropDown.value);
            var restoreAble = new RestoreAbleCollection(saveObjects, type);
            persistenceAPI.SaveObjectPanel(restoreAble);
        });

        importButton = root.Q<Button>("ImportButton");
        importButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            var state = persistenceAPI.LoadObjectPanel<RestoreAbleCollectionState>(Consts.FileExtensions);
            if (state == null || state == default)
            {
                return;
            }
            var loadedCollection = RestoreAbleCollection.Restore<RestoreAbleCollection>(state);
            var toCollection = uASTemplateService.GetCollection(loadedCollection.Type);
            loadedCollection.Models.ForEach(m =>
            {
                toCollection.Add(m as AiObjectModel);
            });
        });


        restoreButton = root.Q<Button>("RestoreButton");
        restoreButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            var state = persistenceAPI.LoadObjectPath<UASTemplateServiceState>(Consts.File_UASTemplateServicelAutoSave);
            if (state == null || state == default)
            {
                return;
            }
            uASTemplateService.Restore(state);
        });

        saveToPlayButton = root.Q<Button>("SaveToPlayButton");
        saveToPlayButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            persistenceAPI.SaveObjectPath(uASTemplateService, Consts.File_PlayAi);
        });


        InitDropdown();
        UpdateLeftPanel();
    }

    private void UpdateButtons()
    {
        createNewButton.style.flexGrow = dropDown.value == Consts.Label_ConsiderationModel ? 0 : 1;
        copyButton.SetEnabled(SelectedModel != null);
        deleteButton.SetEnabled(SelectedModel != null);
    }

    private void AddNewAiObject(string name)
    {
        var aiObject = AssetDatabaseService.CreateInstanceOfType<AiObjectModel>(name);
        uASTemplateService.Add(aiObject);
        ModelSelected(aiObject);
    }

    private void CreateNewModel()
    {
        var s = dropDown.value;
        var type = MainWindowService.GetTypeFromString(s);
        var element = (AiObjectModel)Activator.CreateInstance(type);
        uASTemplateService.Add(element);
        ModelSelected(element);
    }

    private void CopySelectedModel()
    {
        var clone = SelectedModel.Clone();
        uASTemplateService.Add(clone);
        SelectedModel = clone;
    }

    private void DeleteSelectedElement()
    {
        uASTemplateService.Remove(SelectedModel);
        selectedModel = null;
    }

    private void InitDropdown()
    {
        dropDownChoices = new List<string> {
                Consts.Label_UAIModel, 
                Consts.Label_BucketModel, 
                Consts.Label_DecisionModel, 
                Consts.Label_ConsiderationModel,
                Consts.Label_AgentActionModel
            };

        dropDown.label = "Categories";
        dropDown.choices = dropDownChoices;
        dropDown.value = dropDownChoices[0];
        dropDown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            rightPanel.Clear();
            UpdateLeftPanel(evt.newValue);
        });
    }

    public void UpdateLeftPanel(string label = "")
    {
        if (String.IsNullOrEmpty(label))
        {
            label = dropDown.value;
        }
        var models = uASTemplateService.GetCollection(label);
        if (models == null) return;

        activeCollectionChangedSub?.Dispose();
        activeCollectionChangedSub = models
            .OnValueChanged
            .Subscribe(values => LoadModels(values));


        var type = MainWindowService.GetTypeFromString(label);
        var namesFromFiles = AssetDatabaseService.GetActivateableTypes(type);
        addElementPopup.choices = namesFromFiles
            .Where(t => !t.Name.Contains("Mock") && !t.Name.Contains("Stub"))
            .Select(t => t.Name)
            .ToList();


        LoadModels(models.Values);
        UpdateButtons();
    }

    private void LoadModels(List<AiObjectModel> models)
    {
        SelectedModel = null;
        elementsContainer.Clear();
        buttons.Clear();
        selectedObjects.Clear();
        modelsChangedSubsciptions.Clear();

        foreach (var model in models)
        {
            var button = new Button();
            button.text = model.Name;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                ObjectClicked(model, button, evt);
            });
            elementsContainer.Add(button);
            buttons.Add(button);
            model.OnNameChanged
                .Subscribe(newName => button.text = newName)
                .AddTo(modelsChangedSubsciptions);
        }

        var type = MainWindowService.GetTypeFromString(dropDown.value);
        createNewButton.SetEnabled(!type.IsAbstract);
        createNewButton.visible = !type.IsAbstract;
    }

    private List<Button> buttons = new List<Button>();
    private List<KeyValuePair<AiObjectModel,Button>> selectedObjects = new List<KeyValuePair<AiObjectModel, Button>>();

    private void ObjectClicked(AiObjectModel model, Button button, MouseUpEvent e)
    {
        if(e.ctrlKey)
        {
            var isSelected = selectedObjects.FirstOrDefault(o => o.Key == model).Key;
            if (isSelected == null)
            {
                selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(model, button));
            } else
            {
                selectedObjects = selectedObjects.Where(o => o.Key != model).ToList();
            }
        } else if (e.shiftKey)
        {
            var selectedIndex = buttons.IndexOf(button);
            var lowestSelectedIndex = int.MaxValue;
            var highestSelectedIndex = int.MinValue;
            selectedObjects
                .ForEach(pair =>
                {
                    var i = buttons.IndexOf(pair.Value);
                    if (i < lowestSelectedIndex)
                    {
                        lowestSelectedIndex = i;
                    }
                    if (i > highestSelectedIndex)
                    {
                        highestSelectedIndex = i;
                    }
                });
            if (selectedIndex < lowestSelectedIndex)
            {
                selectedObjects.Clear();
                for(var i = selectedIndex; i <= lowestSelectedIndex; i++)
                {
                    var m = uASTemplateService.GetCollection(dropDown.value).Values[i];
                    var b = buttons[i];
                    selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(m, b));
                }
            } else if (selectedIndex > highestSelectedIndex)
            {
                selectedObjects.Clear();
                for(var i = highestSelectedIndex; i <= selectedIndex; i++)
                {
                    var m = uASTemplateService.GetCollection(dropDown.value).Values[i];
                    var b = buttons[i];
                    selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(m, b));
                }
            }
        }
        else
        {
            selectedObjects.Clear();
            selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(model,button));
        }

        SelectedModel = model;
        ModelSelected(model);

        foreach (var b in buttons)
        {
            b.style.color = Color.white;
        }

        foreach(var pair in selectedObjects)
        {
            pair.Value.style.color = Color.gray;
        }
         

    }

    private void ModelSelected(AiObjectModel model)
    {
        if(mainWindowComponent != null)
        {
            mainWindowComponent.Close();
        }
        rightPanel.Clear();
        mainWindowComponent = MainWindowService.GetComponent(model);
        SelectedModel = model;

        rightPanel.Add(mainWindowComponent);
    }

    ~TemplateManager()
    {
        ClearSubscriptions();
    }

    private void OnDestroy()
    {
        persistenceAPI.SaveObjectPath(uASTemplateService, Consts.File_UASTemplateServicelAutoSave);
        ClearSubscriptions();
    }

    private void ClearSubscriptions()
    {
        activeCollectionChangedSub?.Dispose();
        modelsChangedSubsciptions.Clear();
    }
}