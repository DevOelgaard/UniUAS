using System;
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

    private ScoreComponent score;
    public DecisionLogComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        parameters = root.Q<VisualElement>("ParametersContainer");
        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        agentActionsContainer = root.Q<VisualElement>("AgentActionsContainer");

        score = new ScoreComponent(new ScoreModel("Score", 0));
        ScoreContainer.Add(score);

        parametersPool = new LogComponentPool<ParameterLogComponent>(parameters);
        considerationsPool = new LogComponentPool<ConsiderationLogComponent>(considerationsContainer);
        agentActionsPool = new LogComponentPool<AgentActionLogComponent>(agentActionsContainer);
    }

    protected override void DisplayInternal(AiObjectLog aiLog)
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
}