using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class ConsiderationNearestTarget : Consideration
{
    public ConsiderationNearestTarget()
    {
        Min.Value = 0;
        Max.Value = 1000;
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        var agent = (AgentMono)context.Agent;
        var address = context.CurrentEvalutedDecision.GetContextAddress(context);
        var target = context.GetContext<GameObject>(address + AiContextKey.CurrentTargetGameObject);
        
        return Vector3.Distance(agent.transform.position,target.transform.position);
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
