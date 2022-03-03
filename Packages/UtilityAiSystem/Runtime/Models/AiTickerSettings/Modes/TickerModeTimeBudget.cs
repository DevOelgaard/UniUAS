using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class TickerModeTimeBudget : TickerMode
{
    private int lastTickIndex = -1;
    internal TickerModeTimeBudget() : base(AiTickerMode.TimeBudget, Consts.Description_TickerModeTimeBudget)
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
        var endTime = Time.deltaTime + (int)Parameters[0].Value;

        foreach(var agent in agents)
        {
            agent.Tick(metaData);
            
            if (lastTickIndex >= agents.Count)
            {
                lastTickIndex = 0;
            }
            else
            {
                lastTickIndex++;
            }

            if (Time.deltaTime >= endTime)
            {
                break;
            }
        }
    }
}