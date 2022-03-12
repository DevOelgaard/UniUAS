using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentLog
{
    internal AiLog Ai;
    internal string Name;
    internal string Type;

    internal static AgentLog GetDebug(IAgent agent)
    {
        var agentDebug = new AgentLog();
        agentDebug.Name = agent.Model.Name;
        agentDebug.Type = agent.GetType().ToString();

        agentDebug.Ai = AiLog.GetDebug(agent.Ai);
        return agentDebug;
    }
}
