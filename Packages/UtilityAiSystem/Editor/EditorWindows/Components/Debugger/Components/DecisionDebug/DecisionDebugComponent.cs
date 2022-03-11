using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class DecisionDebugComponent : AiObjectDebugComponent
{
    private VisualElement considerationsContainer;
    private VisualElement agentActionsContainer;
    private VisualElement parameters;
    public DecisionDebugComponent(DecisionDebug d) : base(d)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        parameters = root.Q<VisualElement>("ParametersContainer");
        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        agentActionsContainer = root.Q<VisualElement>("AgentActionsContainer");

        foreach(var p in d.Parameters.Parameters)
        {
            parameters.Add(new ParameterDebugComponent(p));
        }

        foreach(var c in d.Considerations)
        {
            considerationsContainer.Add(new ConsiderationDebugComponent(c));
        }

        foreach(var a in d.AgentActions)
        {
            agentActionsContainer.Add(new AgentActionDebugComponent(a));
        }


        var normalizedScore = new ScoreComponent(new ScoreModel("Score", d.Score));
        ScoreContainer.Add(normalizedScore);
    }
}