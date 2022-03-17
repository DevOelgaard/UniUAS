using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_DebugLogParameter : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
             new Parameter("OnStart", "Agent output"),
             new Parameter("OnGoing", "Agent output"),
             new Parameter("OnEnd", "Agent output"),

        };
    }

    public override void OnStart(AiContext context)
    {
        base.OnStart(context);

        Debug.Log("Agent: " + context.Agent.Model.Name + " First tick: " + (string)Parameters[0].Value);
    }

    public override void OnGoing(AiContext context)
    {
        base.OnGoing(context);
        Debug.Log("Agent: " + context.Agent.Model.Name + " continious tick: " + (string)Parameters[1].Value);
    }

    public override void OnEnd(AiContext context)
    {
        base.OnEnd(context);

        Debug.Log("Agent: " + context.Agent.Model.Name + " continious tick: " + (string)Parameters[2].Value);
    }
}