﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class AiLoggerService
{
    private static AiLoggerService instance;
    public static AiLoggerService Instance => instance ??= new AiLoggerService();

    private Dictionary<IAgent, Dictionary<int,AgentLog>> agentLogByAgent = new Dictionary<IAgent, Dictionary<int, AgentLog>>();

    public IObservable<bool> OnTicksChanged => onTicksChanged;
    private Subject<bool> onTicksChanged = new Subject<bool>();
    public int MinTick { get; private set; } = int.MaxValue;
    public int MaxTick { get; private set; } = int.MinValue;


    public AiLoggerService()
    {
    }

    public void Clear()
    {
        agentLogByAgent.Clear();
        MinTick = int.MaxValue;
        MaxTick = int.MinValue;
        onTicksChanged.OnNext(true);
    }

    public void LogTick(IAgent agent, int tick)
    {
        var agentLog = AgentLog.GetDebug(agent);
        if (agentLog == null) return;
        if (tick < MinTick)
        {
            MinTick = tick;
            onTicksChanged.OnNext(true);
        }
        if (tick > MaxTick)
        {
            MaxTick = tick;
            onTicksChanged.OnNext(true);
        }

        if (agentLogByAgent.ContainsKey(agent))
        {
            if (agentLogByAgent[agent].ContainsKey(tick))
            {
                agentLogByAgent[agent][tick] = agentLog;
            } else
            {
                agentLogByAgent[agent].Add(tick, agentLog);
            }
        } else
        {
            agentLogByAgent.Add(agent, new Dictionary<int,AgentLog>());
            agentLogByAgent[agent].Add(tick, agentLog);
        }
    }


    public AgentLog GetAiDebugLog(IAgent agent, int tick)
    {
        if (agent == null) return null;
        if (tick == default) return null;
        if (!agentLogByAgent.ContainsKey(agent)) return null;
        if (!agentLogByAgent[agent].ContainsKey(tick)) return null;
        return agentLogByAgent[agent][tick];
    }
}