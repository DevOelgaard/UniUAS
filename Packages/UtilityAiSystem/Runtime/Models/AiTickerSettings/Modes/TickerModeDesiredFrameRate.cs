using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class TickerModeDesiredFrameRate : TickerMode
{
    private int frameCount = 0;
    private float timeCount = 0f;
    private int tickedItemsLastSample = 0;
    private int allowedTicksPrFrame = int.MaxValue;
    private int lastTickIndex = -1;
    internal float LastFrameRate { get; private set; } = 0f;
    internal float SampelTimeInSeconds => Convert.ToSingle(Parameters[1].Value);
    internal float TargetFrameRate => Convert.ToSingle(Parameters[0].Value);

    public TickerModeDesiredFrameRate() : base(AiTickerMode.DesiredFrameRate, Consts.Description_TickerModeDesiredFrameRate)
    {
    }

    internal override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        result.Add(new Parameter("Target Framerate", (int)60));
        result.Add(new Parameter("Sample Time S", 1f));
        return result;
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        if (timeCount < SampelTimeInSeconds)
        {
            timeCount += Time.deltaTime;
            frameCount++;
        }
        else
        {
            LastFrameRate = (float)frameCount / timeCount;
            frameCount = 0;
            timeCount = 0f;
        }

        if (LastFrameRate < TargetFrameRate)
        {
            var optimizeFactor = LastFrameRate / TargetFrameRate;
            allowedTicksPrFrame = Mathf.FloorToInt(tickedItemsLastSample * optimizeFactor);
        }

        var tickCountThisFrame = 0;
        foreach(var agent in agents)
        {
            tickCountThisFrame++;
            if (lastTickIndex >= agents.Count)
            {
                lastTickIndex = 0;
            } else
            {
                lastTickIndex++;
            }
            agents[lastTickIndex].Tick(metaData);

            
            if (tickCountThisFrame >= allowedTicksPrFrame)
            {
                break;
            }
        }
    }
}