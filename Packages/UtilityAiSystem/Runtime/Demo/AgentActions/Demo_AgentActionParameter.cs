using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_AgentActionParameter : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        var p1 = new Parameter("Output", "Agent output");
        result.Add(p1);

        return result;
    }

    public override void OnStart(AiContext context)
    {
        base.OnStart(context);

        Debug.Log("Agent: " + context.Agent.Model.Name + " " + (string)Parameters[0].Value);
    }
}