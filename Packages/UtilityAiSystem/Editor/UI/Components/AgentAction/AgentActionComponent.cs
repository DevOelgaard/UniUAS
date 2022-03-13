﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

internal class AgentActionComponent : MainWindowComponent
{
    private TemplateContainer root;
    private VisualElement parametersContainer;
    private AgentAction agentAction;
    internal AgentActionComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("ParametersContainer");

        Body.Clear();
        Body.Add(root);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var agentAction = model as AgentAction;
        this.agentAction = agentAction;
        SetParameters();

    }

    private void SetParameters()
    {
        Debug.LogWarning("This could be more effective by using a pool");
        parametersContainer.Clear();
        foreach (var parameter in agentAction.Parameters)
        {
            parametersContainer.Add(new ParameterComponent(parameter));
        }
    }
}

