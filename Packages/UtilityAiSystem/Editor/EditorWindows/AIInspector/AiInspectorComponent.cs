using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;

public class AiInspectorComponent : EditorWindow
{
    private CompositeDisposable agentsChangedSubscription = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftContainer;
    private VisualElement rightContainer;
    private VisualElement buttonContainer;
    
    private DropdownField agentTypeDropdown;

    private IDisposable agentsChangedSub;
    private IDisposable agentTypesChangedSub;
    private IAgent selectedAgent;

    private AgentManager agentManager => AgentManager.Instance;


    [MenuItem(Statics.MenuName + Statics.Name_AiInspector)]
    public static void Open()
    {
        AiInspectorComponent wnd = GetWindow<AiInspectorComponent>();
        wnd.titleContent = new GUIContent(Statics.Name_AiInspector);
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

        agentTypeDropdown = root.Q<DropdownField>("AgentType-Dropdown");

        agentTypesChangedSub = agentManager
            .AgentTypesUpdated
            .Subscribe(_ => InitDropDown());

        InitDropDown();


    }

    private void InitDropDown()
    {
        agentTypeDropdown.choices = agentManager.AgentTypes;
        agentTypeDropdown.label = "Agent Types";
        if (agentManager.AgentTypes.Count > 0)
        {
            agentTypeDropdown.value = agentTypeDropdown.choices.First();
        } else
        {
            agentTypeDropdown.value = "No agents found";
        }

        agentTypeDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            UpdateLeftPanel(evt.newValue);
        });
    }

    private void UpdateLeftPanel(string newValue)
    {
        var agents = agentManager.GetAgentsByType(newValue);

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
        agentsChangedSubscription?.Clear();

        foreach(var agent in agents)
        {
            var button = new Button();
            button.text = agent.Model.Name;
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
                .AddTo(agentsChangedSubscription);
        }
    }


    private void SelectedAgentChanged()
    {
        throw new NotImplementedException();
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
        agentsChangedSubscription.Clear();
    }

    ~AiInspectorComponent()
    {
        ClearSubscriptions();
    }

}
