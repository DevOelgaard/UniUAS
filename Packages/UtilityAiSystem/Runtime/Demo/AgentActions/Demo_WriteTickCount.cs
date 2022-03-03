using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_WriteTickCount : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public override void OnStart(AiContext context)
    {
        base.OnStart(context);
        var value = context.GetContext<int>(AiContextKey.TickValue_INT);
        Debug.Log("Agent: " + context.Agent.Model.Name + " TickCount: " + value); ;
    }
}
