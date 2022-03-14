using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentLog: AiObjectLog
{
    internal AiLog Ai;

    internal static AgentLog GetDebug(IAgent agent)
    {
        if (agent == null) return null;
        var agentDebug = new AgentLog();
        agentDebug.Name = agent.Model.Name;
        agentDebug.Type = agent.GetType().ToString();

        agentDebug.Ai = AiLog.GetDebug(agent.Ai);
        return agentDebug;
    }
}
