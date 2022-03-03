using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class TickerModeMultiThread : TickerMode
{
    public TickerModeMultiThread() : base(AiTickerMode.MultiThread, Consts.Description_TickerModeMultiThread)
    {
    }

    internal override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        result.Add(new Parameter("Threads", (int)1));
        return result;
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        throw new NotImplementedException();
    }
}
