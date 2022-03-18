using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentLog: AiObjectLog
{
    internal AiLog Ai;

    internal static AgentLog GetDebug(IAgent agent, int tick)
    {
        if (agent == null) return null;
        var result = new AgentLog();
        result = SetBasics(result, agent, tick) as AgentLog;
        result.Ai = AiLog.GetDebug(agent.Ai, tick);
        return result;
    }
}
