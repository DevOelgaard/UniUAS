using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

internal class AgentActionComponent : MainWindowComponent
{
    private TemplateContainer root;
    private VisualElement parametersContainer;
    private AgentAction agentAction;
    internal AgentActionComponent(AgentAction agentAction) : base(agentAction)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("ParametersContainer");
        this.agentAction = agentAction;

        Body.Clear();
        Body.Add(root);

        SetParameters();
    }

    private void SetParameters()
    {
        parametersContainer.Clear();
        foreach (var parameter in agentAction.Parameters)
        {
            parametersContainer.Add(new ParameterComponent(parameter));
        }
    }
}

