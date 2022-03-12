﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentDebug
{
    internal AiLog Ai;
    internal string Name;
    internal string Type;

    internal static AgentDebug GetDebug(IAgent agent)
    {
        var agentDebug = new AgentDebug();
        agentDebug.Name = agent.Model.Name;
        agentDebug.Type = agent.GetType().ToString();

        agentDebug.Ai = AiLog.GetDebug(agent.Ai);
        return agentDebug;
    }
}
