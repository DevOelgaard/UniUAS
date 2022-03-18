using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_DebugObjectFromParam : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Consideration", typeof(Consideration))
        };
    }

    public override void OnGoing(AiContext context)
    {
        Debug.Log("O: " + Parameters[0].Value.GetType());
    }
}
