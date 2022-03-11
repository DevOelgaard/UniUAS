using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionDebug: AiObjectDebug
{
    internal ParametersDebug Parameters;

    internal static AgentActionDebug GetActionDebug(AgentAction aa)
    {
        var result = new AgentActionDebug();
        result.Parameters = new ParametersDebug(aa.Parameters);
        return result;
    }
}
