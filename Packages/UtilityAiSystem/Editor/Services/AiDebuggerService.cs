using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class AiDebuggerService
{
    private static AiDebuggerService instance;
    public static AiDebuggerService Instance => instance ??= new AiDebuggerService();

    private Dictionary<IAgent, Dictionary<int,AiDebug>> aiModelsByAgent = new Dictionary<IAgent, Dictionary<int, AiDebug>>();

    public IObservable<bool> OnTicksChanged => onTicksChanged;
    private Subject<bool> onTicksChanged = new Subject<bool>();
    public int MinTick { get; private set; } = int.MaxValue;
    public int MaxTick { get; private set; } = int.MinValue;


    public AiDebuggerService()
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
            aiModelsByAgent.Add(agent, new Dictionary<int,AiDebug>());
            aiModelsByAgent[agent].Add(tick, aiModel);
        }
    }


    public AiDebug GetAiDebugLog(IAgent agent, int tick)
    {
        if (aiModelsByAgent.ContainsKey(agent) && aiModelsByAgent[agent].ContainsKey(tick))
        {
            return aiModelsByAgent[agent][tick];
        }
        return null;
    }
}