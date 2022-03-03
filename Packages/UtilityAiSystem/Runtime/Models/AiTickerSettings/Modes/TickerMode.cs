using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal abstract class TickerMode
{
    internal AiTickerMode Name;
    internal string Description;
    internal List<Parameter> Parameters;

    protected TickerMode(AiTickerMode name, string description)
    {
        Description = description;
        Name = name;
        Parameters = GetParameters();
    }



    internal abstract List<Parameter> GetParameters();
    internal abstract void Tick(List<IAgent> agents, TickMetaData metaData);
}
