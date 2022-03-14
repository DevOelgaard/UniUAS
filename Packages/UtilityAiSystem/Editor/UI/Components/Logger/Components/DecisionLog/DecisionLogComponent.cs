﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class DecisionLogComponent : AiObjectLogComponent
{
    private VisualElement considerationsContainer;
    private VisualElement agentActionsContainer;
    private VisualElement parameters;
    private LogComponentPool<ParameterLogComponent> parametersPool;
    private LogComponentPool<ConsiderationLogComponent> considerationsPool;
    private LogComponentPool<AgentActionLogComponent> agentActionsPool;

    private ScoreLogComponent score;
    public DecisionLogComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        parameters = root.Q<VisualElement>("ParametersContainer");
        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        agentActionsContainer = root.Q<VisualElement>("AgentActionsContainer");

        score = new ScoreLogComponent("Score", 0.ToString());
        ScoreContainer.Add(score);

        parametersPool = new LogComponentPool<ParameterLogComponent>(parameters);
        considerationsPool = new LogComponentPool<ConsiderationLogComponent>(considerationsContainer,3);
        agentActionsPool = new LogComponentPool<AgentActionLogComponent>(agentActionsContainer,1);
    }

    protected override void UpdateUiInternal(AiObjectLog aiLog)
    {
        var d = aiLog as DecisionLog;
        var logModels = new List<ILogModel>();
        foreach (var p in d.Parameters)
        {
            logModels.Add(p);
        }
        parametersPool.Display(logModels);

        logModels.Clear();
        foreach (var c in d.Considerations)
        {
            logModels.Add(c);
        }
        considerationsPool.Display(logModels);

        logModels.Clear();
        foreach (var a in d.AgentActions)
        {
            logModels.Add(a);
        }
        agentActionsPool.Display(logModels);

        score.UpdateScore(d.Score);
    }

    internal override void Hide()
    {
        base.Hide();
        parametersPool.Hide();
        considerationsPool.Hide();
        agentActionsPool.Hide();
    }

    internal override void SetColor()
    {
        base.SetColor();

        var list = new List<KeyValuePair<VisualElement, float>>();
        foreach (var c in considerationsPool.LogComponents)
        {
            if (c.Model == null) continue;
            var cast = c.Model as ConsiderationLog;
            list.Add(new KeyValuePair<VisualElement, float>(c, cast.NormalizedScore));
        }
        ColorService.SetColor(list);
    }
}