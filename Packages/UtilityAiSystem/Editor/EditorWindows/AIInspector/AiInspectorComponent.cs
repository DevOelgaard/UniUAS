using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;

public class AiInspectorComponent : EditorWindow
{
    private CompositeDisposable agentNameUpdatedSubscriptions = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftContainer;
    private VisualElement rightContainer;
    private VisualElement buttonContainer;
    private AgentComponent agentComponent;
    
    private DropdownField identifierDropdown;

    private IDisposable agentsChangedSub;
    private IDisposable agentTypesChangedSub;
    private IDisposable agentCollectionUpdatedSub;
    private IAgent selectedAgent;

    private AgentManager agentManager => AgentManager.Instance;


    [MenuItem(Consts.MenuName + Consts.Name_AiInspector)]
    public static void Open()
    {
        AiInspectorComponent wnd = GetWindow<AiInspectorComponent>();
        wnd.titleContent = new GUIContent(Consts.Name_AiInspector);
        wnd.Show();
        wnd.position = new Rect(0f, 0f, 1920 / 3, 1080 / 2);
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);

        leftContainer = root.Q<VisualElement>("LeftContainer");
        rightContainer = root.Q<VisualElement>("RightContainer");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");

        identifierDropdown = root.Q<DropdownField>("AgentType-Dropdown");

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

    private void InitDropDown()
    {
        identifierDropdown.choices = agentManager.AgentIdentifiers;
        identifierDropdown.choices.Add("Demo");
        identifierDropdown.label = "Agent Types";
        if (agentManager.AgentIdentifiers.Count > 0)
        {
            identifierDropdown.value = identifierDropdown.choices.First();
        } else
        {
            identifierDropdown.value = "No agents found";
        }

        identifierDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            UpdateLeftPanel(evt.newValue);
        });
    }

    private void UpdateLeftPanel(string identifier = null)
    {
        if (identifier == null)
        {
            identifier = identifierDropdown.value;
        }
        var agents = agentManager.GetAgentsByIdentifier(identifier);

        agentsChangedSub?.Dispose();
        agentsChangedSub = agents
            .OnValueChanged
            .Subscribe(values =>
            {
                LoadAgents(values);
            });

        LoadAgents(agents.Values);
    }

    private void LoadAgents(List<IAgent> agents)
    {
        SelectedAgent = null;
        buttonContainer.Clear();
        agentNameUpdatedSubscriptions?.Clear();

        foreach(var agent in agents)
        {
            var button = new Button();
            button.text = agent.Model.Name;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                SelectedAgent = agent;
            });
            buttonContainer.Add(button);
            agent
                .Model
                .OnNameChanged
                .Subscribe(name =>
                {
                    button.text = name;
                })
                .AddTo(agentNameUpdatedSubscriptions);
        }
    }

    private void SelectedAgentChanged()
    {
        rightContainer.Clear();
        if (SelectedAgent == null) return;

        agentComponent = new AgentComponent(SelectedAgent);
        rightContainer.Add(agentComponent);
    }

    private IAgent SelectedAgent
    {
        get => selectedAgent;
        set
        {
            selectedAgent = value;
            SelectedAgentChanged();
        }
    }

    void ClearSubscriptions()
    {
        agentsChangedSub?.Dispose();
        agentTypesChangedSub?.Dispose();
        agentCollectionUpdatedSub?.Dispose(); 
        agentNameUpdatedSubscriptions.Clear();
    }

    ~AiInspectorComponent()
    {
        ClearSubscriptions();
    }

}
