using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionDebugComponent : AiObjectDebugComponent
{
    public AgentActionDebugComponent(AgentActionDebug a) : base(a)
    {
        foreach(var p in a.Parameters.Parameters)
        {
            Body.Add(new ParameterDebugComponent(p));
        }
    }
}
