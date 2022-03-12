using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionLogComponent : AiObjectLogComponent
{
    private LogComponentPool<ParameterLogComponent> parameterPool;
    public AgentActionLogComponent() : base()
    {
        parameterPool = new LogComponentPool<ParameterLogComponent>(Body);
    }

    protected override void DisplayInternal(AiObjectLog aiObjectDebug)
    {
        var a = aiObjectDebug as AgentActionDebug;

        var logModels = new List<ILogModel>();
        foreach (var p in a.Parameters)
        {
            logModels.Add(p);
        }
        parameterPool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        parameterPool.Hide();
    }
}
