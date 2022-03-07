using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;
using MoreLinq;

internal class AgentComponent: VisualElement
{
    private TemplateContainer root;
    private VisualElement body;
    private Label agentName;
    private DropdownField aiDropdown;
    private UAIComponent aiComponent;
    private VisualElement footer;
    private Button tickAgent;
    private Button tickAllButton;
    private Button applyToAllButton;
    private IAgent agent;
    private UASTemplateService uasTemplateService;

    internal AgentComponent(IAgent agent)
    {
        this.agent = agent;
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");
        footer = root.Q<VisualElement>("Footer");

        uasTemplateService = UASTemplateService.Instance;

        tickAgent = new Button();
        tickAgent.text = "TEST-Tick-Agent";
        tickAgent.RegisterCallback<MouseUpEvent>(evt =>
        {
            var metaData = new TickMetaData();
            metaData.TickedBy = "Agent: " + agent.Model.Name;
            agent.Tick(metaData);
        });
        footer.Add(tickAgent);

        tickAllButton = new Button();
        tickAllButton.text = "TEST-Tick-All";
        tickAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AiTicker.Instance.TickAis();
        });
        footer.Add(tickAllButton);

        applyToAllButton = new Button();
        applyToAllButton.text = "Apply to all";
        applyToAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AgentManager.Instance.GetAgentsByIdentifier(agent.TypeIdentifier).Values
                .ForEach(a =>
                {
                    var aiClone = agent.Ai.Clone() as Ai;
                    a.SetAi(aiClone);
                });

        });
        footer.Add(applyToAllButton);

        agentName.text = agent.Model.Name;

        InitDropdown();

        UpdateAiComponent();
    }

    private void InitDropdown()
    {
        aiDropdown.label = "AIs";
        uasTemplateService.LoadPlayMode();
        aiDropdown.choices = uasTemplateService
            .GetCollection(Consts.Label_UAIModel)
            .Values
            .Select(x => x.Name)
            .ToList();

        if (agent.Ai != null && aiDropdown.choices.Contains(agent.Ai.Name))
        {
            aiDropdown.value = agent.Ai?.Name;
        }

        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            agent.Ai = UASTemplateService.Instance.GetAiByName(evt.newValue);
            UpdateAiComponent();
        });
    }

    private void UpdateAiComponent()
    {
        body.Clear();

        if (agent.Ai == null) return;
        aiComponent = new UAIComponent(agent.Ai);
        body.Add(aiComponent);
    }
}
