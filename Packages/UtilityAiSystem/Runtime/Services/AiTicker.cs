using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using MoreLinq;

internal class AiTicker
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private static AiTicker instance;
    public static AiTicker Instance => instance ?? (instance = new AiTicker());
    private AgentManager agentManager => AgentManager.Instance;
    private int tickCount = 0;

    internal AiTicker()
    {
        Observable.IntervalFrame(120)
            .Subscribe(_ => TickAis())
            .AddTo(disposables);
    }

    internal void Start()
    {

    }

    private void TickAis()
    {
        tickCount++;
        agentManager.Model.Agents.Values
            .ForEach(agent =>
            {
                agent.Model.AI.Context.SetContext(AiContextKey.TickValue_INT,tickCount);
                agent.Tick();
            });
    }
}
