using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class TickerModeDesiredFrameRate : TickerMode
{
    public TickerModeDesiredFrameRate() : base(AiTickerMode.DesiredFrameRate, Consts.Description_TickerModeMultiThread)
    {
    }

    internal override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        result.Add(new Parameter("Target Framerate", (int)60));
        return result;
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        throw new NotImplementedException();
    }
}