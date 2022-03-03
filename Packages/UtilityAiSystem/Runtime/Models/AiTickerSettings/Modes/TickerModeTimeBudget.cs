using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class TickerModeTimeBudget : TickerMode
{
    public TickerModeTimeBudget() : base(AiTickerMode.TimeBudget, Consts.Description_TickerModeMultiThread)
    {
    }

    internal override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        result.Add(new Parameter("Time Budget MS", (int)23));
        return result;
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        throw new NotImplementedException();
    }
}