using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionDebug: AiObjectLog
{
    internal List<ParameterLog> Parameters;

    internal static AgentActionDebug GetActionDebug(AgentAction aa)
    {
        var result = new AgentActionDebug();
        result.Parameters = new List<ParameterLog>();
        foreach (var p in aa.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p));
        }
        return result;
    }
}
