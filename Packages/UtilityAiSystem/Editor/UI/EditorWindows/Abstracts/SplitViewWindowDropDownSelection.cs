using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;

internal abstract class SplitViewWindowDropDownSelection<T> : EditorWindow
{
    private CompositeDisposable elementNameUpdatedSub = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftContainer;
    private VisualElement rightContainer;
    private VisualElement buttonContainer;
    //private DebuggerComponent debuggerComponent;
    private RightPanelComponent<T> rightPanelComponent;

    private DropdownField identifierDropdown;

    private IDisposable elementChangedSub;
    private IDisposable agentTypesChangedSub;
    private IDisposable agentCollectionUpdatedSub;
    private T selectedElement;

    private AgentManager agentManager => AgentManager.Instance;


    //[MenuItem(Consts.MenuName + Consts.Name_AiInspector)]
    //public static void Open()
    //{
    //    SplitViewWindowDropDownSelection<T> wnd = GetWindow<SplitViewWindowDropDownSelection<T>>();
    //    wnd.titleContent = new GUIContent(Consts.Name_AiInspector);
    //    wnd.Show();
    //    wnd.position = new Rect(0f, 0f, 1920 / 3, 1080 / 2);
    //}

    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);

        leftContainer = root.Q<VisualElement>("LeftContainer");
        rightContainer = root.Q<VisualElement>("RightContainer");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");

        identifierDropdown = root.Q<DropdownField>("AgentType-Dropdown");
        rightPanelComponent = GetRightPanelComponent();
        rightContainer.Add(rightPanelComponent);

        agentTypesChangedSub = agentManager
            .AgentTypesUpdated
            .Subscribe(_ => InitDropDown());

        InitDropDown();

        UpdateLeftPanel();

        agentCollectionUpdatedSub = agentManager
            .AgentsUpdated
            .Subscribe(agent =>
            {
                if (agent.TypeIdentifier == identifierDropdown.value)
                {
                    UpdateLeftPanel();
                }
            });

    }

    protected abstract RightPanelComponent<T> GetRightPanelComponent();

    private void InitDropDown()
    {
        identifierDropdown.choices = agentManager.AgentIdentifiers;
        identifierDropdown.choices.Add("Demo");
        identifierDropdown.label = "Agent Types";
        if (agentManager.AgentIdentifiers.Count > 0)
        {
            identifierDropdown.value = identifierDropdown.choices.First();
        }
        else
        {
            identifierDropdown.value = "No agents found";
        }

        identifierDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            UpdateLeftPanel(evt.newValue);
        });
    }

    protected abstract ReactiveList<T> GetLeftPanelElements(string identifier);

    private void UpdateLeftPanel(string identifier = null)
    {
        if (identifier == null)
        {
            identifier = identifierDropdown.value;
        }
        var elements = GetLeftPanelElements(identifier);

        elementChangedSub?.Dispose();
        elementChangedSub = elements
            .OnValueChanged
            .Subscribe(values =>
            {
                LoadElements(values);
            });

        LoadElements(elements.Values);
    }

    private void LoadElements(List<T> elements)
    {
        SelectedElement = default(T);
        buttonContainer.Clear();
        elementNameUpdatedSub?.Clear();

        foreach (var e in elements)
        {
            var button = new Button();
            button.text = GetNameFromElement(e);
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                SelectedElement = e;
            });

            buttonContainer.Add(button);
            //e.OnNameChanged
            //    .Subscribe(name =>
            //    {
            //        button.text = name;
            //    })
            //    .AddTo(elementNameUpdatedSub);
        }
    }

    protected abstract string GetNameFromElement(T element);

    private void SelectedAgentChanged()
    {
        rightPanelComponent.UpateUi(SelectedElement);
    }

    private T SelectedElement
    {
        get => selectedElement;
        set
        {
            selectedElement = value;
            SelectedAgentChanged();
        }
    }

    void ClearSubscriptions()
    {
        elementChangedSub?.Dispose();
        agentTypesChangedSub?.Dispose();
        agentCollectionUpdatedSub?.Dispose();
        elementNameUpdatedSub.Clear();
    }

    private void OnDestroy()
    {
        var persistenceAPI = new PersistenceAPI(new JSONPersister());
        persistenceAPI.SaveObjectPath(UASTemplateService.Instance, Consts.File_UASTemplateServicelAutoSave);
        ClearSubscriptions();
    }

    ~SplitViewWindowDropDownSelection()
    {
        ClearSubscriptions();
    }
}