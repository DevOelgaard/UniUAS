using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

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
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");

        agentName.text = agent.Model.Name;

        aiDropdown.value = agent.Model.AI.Name;
        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            throw new NotImplementedException();
        });

        UpdateAiComponent();
    }

    private void UpdateAiComponent()
    {
        aiComponent = new UAIComponent(agent.Model.AI);
        body.Clear();
        body.Add(aiComponent);
    }
}
