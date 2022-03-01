using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;

public class AgentComponent: VisualElement
{
    private TemplateContainer root;
    private VisualElement body;
    private Label agentName;
    private DropdownField aiDropdown;
    private UAIComponent aiComponent;
    private IAgent agent;

    public AgentComponent(IAgent agent)
    {
        this.agent = agent;
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");

        agentName.text = agent.Model.Name;

        InitDropdown();

        UpdateAiComponent();
    }

    private void InitDropdown()
    {
        aiDropdown.label = "AIs";
        aiDropdown.choices = UASTemplateService
            .Instance
            .GetCollection(Statics.Label_UAIModel)
            .Values
            .Select(x => x.Name)
            .ToList();

        if (agent.Model.AI != null && aiDropdown.choices.Contains(agent.Model.AI.Name))
        {
            aiDropdown.value = agent.Model.AI?.Name;
        }

        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            agent.Model.AI = UASTemplateService.Instance.GetAiByName(evt.newValue);
            UpdateAiComponent();
        });
    }

    private void UpdateAiComponent()
    {
        body.Clear();

        if (agent.Model.AI == null) return;
        aiComponent = new UAIComponent(agent.Model.AI);
        body.Add(aiComponent);
    }
}
