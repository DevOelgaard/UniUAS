using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class AiLoggerService
{
    private static AiLoggerService instance;
    public static AiLoggerService Instance => instance ??= new AiLoggerService();

    private Dictionary<IAgent, Dictionary<int,AiLog>> aiModelsByAgent = new Dictionary<IAgent, Dictionary<int, AiLog>>();

    public IObservable<bool> OnTicksChanged => onTicksChanged;
    private Subject<bool> onTicksChanged = new Subject<bool>();
    public int MinTick { get; private set; } = int.MaxValue;
    public int MaxTick { get; private set; } = int.MinValue;


    public AiLoggerService()
    {
    }

    public void Clear()
    {
        aiModelsByAgent.Clear();
        MinTick = int.MaxValue;
        MaxTick = int.MinValue;
        onTicksChanged.OnNext(true);
    }

    public void LogTick(IAgent agent, int tick)
    {
        var aiModel = AgentDebug.GetDebug(agent).Ai;
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

        if (aiModelsByAgent.ContainsKey(agent))
        {
            if (aiModelsByAgent[agent].ContainsKey(tick))
            {
                aiModelsByAgent[agent][tick] = aiModel;
            } else
            {
                aiModelsByAgent[agent].Add(tick, aiModel);
            }
        } else
        {
            aiModelsByAgent.Add(agent, new Dictionary<int,AiLog>());
            aiModelsByAgent[agent].Add(tick, aiModel);
        }
    }


    public AiLog GetAiDebugLog(IAgent agent, int tick)
    {
        if (aiModelsByAgent.ContainsKey(agent) && aiModelsByAgent[agent].ContainsKey(tick))
        {
            return aiModelsByAgent[agent][tick];
        }
        return null;
    }
}