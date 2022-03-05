using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class TickerModeTimeBudget : TickerMode
{
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private int lastTickIndex = -1;
    internal TickerModeTimeBudget() : base(AiTickerMode.TimeBudget, Consts.Description_TickerModeTimeBudget)
    {
    }

    internal override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Time Budget MS", (int)23),
            new Parameter("Debug", false),
        };
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        stopwatch.Reset();
        stopwatch.Start();
        var tickedAgents = 0;
        foreach(var agent in agents)
        {
            if (stopwatch.ElapsedMilliseconds >= Convert.ToSingle(Parameters[0].Value))
            {
                if ((bool)Parameters[1].Value)
                {
                    Debug.Log("Breaking tickedAgents: " + tickedAgents + " Elapsed Time: " + stopwatch.ElapsedMilliseconds + "ms");
                }
                if(tickedAgents <= 0)
                {
                    Debug.LogWarning("No agents ticked. The time budget may be to low! Consider increasing the Time budget or the performance of the active agents!");
                }
                break;
            }

            agent.Tick(metaData);
            
            if (lastTickIndex >= agents.Count-1)
            {
                lastTickIndex = 0;
            }
            else
            {
                lastTickIndex++;
            }
            tickedAgents++;
            if(tickedAgents >= agents.Count)
            {
                if ((bool)Parameters[1].Value)
                {
                    Debug.Log("All agents ticked agents.count: " + agents.Count + " Elapsed Time: " + stopwatch.ElapsedMilliseconds + "ms");
                }
                break;
            }


        }
        stopwatch.Stop();
    }
}