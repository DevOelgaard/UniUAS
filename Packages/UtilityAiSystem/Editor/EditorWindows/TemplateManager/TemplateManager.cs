using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UniRx;


internal class TemplateManager : EditorWindow
{
    private IDisposable activeCollectionChangedSub;

    private CompositeDisposable modelsChangedSubsciptions = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftPanel;
    private VisualElement buttonContainer;
    private VisualElement rightPanel;
    private Button createNewButton;
    private Button copyButton;
    private Button deleteButton;
    private Button resetButton;
    private Button saveButton;
    private Button loadButton;
    private Button refreshButton;
    private UASTemplateService uASModel => UASTemplateService.Instance;

    private MainWindowComponent mainWindowComponent;
    private AiObjectModel selectedModel;
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
        root = rootVisualElement;

        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);
        dropDown = root.Q<DropdownField>("TypeDropdown");

        leftPanel = root.Q<VisualElement>("left-panel");
        buttonContainer = leftPanel.Q<VisualElement>("ButtonContainer");

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
            uASModel.Reset();
            UpdateLeftPanel();
            rightPanel.Clear();
        });

        saveButton = root.Q<Button>("SaveButton");
        saveButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uASModel.SaveToFile();
        });

        loadButton = root.Q<Button>("LoadButton");
        loadButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uASModel.LoadFromFile();
            UpdateLeftPanel();
        });

        refreshButton = root.Q<Button>("RefreshButton");
        refreshButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uASModel.Refresh();
            UpdateLeftPanel();
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

    private void CreateNewModel()
    {
        var s = dropDown.value;
        var type = MainWindowService.GetTypeFromString(s);
        var element = (AiObjectModel)Activator.CreateInstance(type);
        uASModel.Add(element);
        ModelSelected(element);
    }

    private void CopySelectedModel()
    {
        var clone = SelectedModel.Clone();
        uASModel.Add(clone);
        SelectedModel = clone;
    }

    private void DeleteSelectedElement()
    {
        uASModel.Remove(SelectedModel);
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
        if (label == "")
        {
            label = dropDown.value;
        }
        var models = uASModel.GetCollection(label);

        activeCollectionChangedSub?.Dispose();
        activeCollectionChangedSub = models
            .OnValueChanged
            .Subscribe(values => LoadModels(values));

        LoadModels(models.Values);
        UpdateButtons();
    }

    private void LoadModels(List<AiObjectModel> models)
    {
        SelectedModel = null;
        buttonContainer.Clear();
        modelsChangedSubsciptions.Clear();

        foreach (var model in models)
        {
            var button = new Button();
            button.text = model.Name;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                SelectedModel = model;
                ModelSelected(model);
            });
            buttonContainer.Add(button);
            model.OnNameChanged
                .Subscribe(newName => button.text = newName)
                .AddTo(modelsChangedSubsciptions);
        }

        var type = MainWindowService.GetTypeFromString(dropDown.value);
        createNewButton.SetEnabled(!type.IsAbstract);
        createNewButton.visible = !type.IsAbstract;
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
        ClearSubscriptions();
    }

    private void ClearSubscriptions()
    {
        activeCollectionChangedSub?.Dispose();
        modelsChangedSubsciptions.Clear();
    }
}