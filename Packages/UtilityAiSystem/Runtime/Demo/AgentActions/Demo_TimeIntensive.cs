using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_TimeIntensive : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>() { 
            new Parameter("ExecutionTime ms", 3) 
        };
    }

    public override void OnStart(AiContext context)
    {
        var end = Time.deltaTime + Convert.ToInt32(Parameters[0].Value);

        while(end > Time.deltaTime)
        {

        }
    }
}
