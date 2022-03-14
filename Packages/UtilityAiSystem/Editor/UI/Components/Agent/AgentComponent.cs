using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;
using MoreLinq;

internal class AgentComponent: RightPanelComponent<IAgent>
{
    private TemplateContainer root;
    private VisualElement body;
    private Label agentName;
    private DropdownField aiDropdown;
    private AiComponent aiComponent;
    private VisualElement footer;
    private Button tickAgent;
    private Button tickAllButton;
    private Button applyToAllButton;
    private IAgent agent;
    private UASTemplateService uasTemplateService;

    internal AgentComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");
        footer = root.Q<VisualElement>("Footer");
        aiComponent = new AiComponent();
        body.Add(aiComponent);

        uasTemplateService = UASTemplateService.Instance;

        tickAgent = new Button();
        tickAgent.text = "TEST-Tick-Agent";
        tickAgent.RegisterCallback<MouseUpEvent>(evt =>
        {
            AiTicker.Instance.TickAgent(agent);
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
    }

    internal override void UpateUi(IAgent element)
    {
        if (element == null) return;
        this.agent = element;
        agentName.text = agent.Model.Name;
        InitDropdown();

        UpdateAiComponent();
    }

    private void InitDropdown()
    {
        aiDropdown.label = "AIs";
        uasTemplateService.LoadAutoSave();
        aiDropdown.choices = uasTemplateService
            .GetCollection(Consts.Label_UAIModel)
            .Values
            .Cast<Ai>()
            .Where(ai => ai.IsPLayable)
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

    internal void UpdateAiComponent()
    {
        aiComponent.UpdateUi(agent.Ai);
    }

}
