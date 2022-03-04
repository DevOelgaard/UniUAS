using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

internal class TickerModeDesiredFrameRate : TickerMode
{
    private CompositeDisposable disposables = new CompositeDisposable();
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
        return new List<Parameter>()
        {
            new Parameter("Target Framerate", (int)60),
            new Parameter("Sample Time Seconds", 1f),
            new Parameter("Debug", false)
        };
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
        if ((bool)Parameters[2].Value)
        {
            Debug.Log("Framerate: " + LastFrameRate + " Allowed TicksPrFrame: " + allowedTicksPrFrame);
        }
    }

    ~TickerModeDesiredFrameRate()
    {
        disposables.Clear();
    }
}