using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_TimeIntensive : AgentAction
{
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>() { 
            new Parameter("ExecutionTime ms", 3) 
        };
    }

    public override void OnStart(AiContext context)
    {
        stopwatch.Reset();
        stopwatch.Start();
        var time = stopwatch.ElapsedMilliseconds;
        var extra = Convert.ToInt32(Parameters[0].Value);
        var end = time + extra;

        var i = 0;
        //Debug.Log("Time: " + time + " extra: " + extra + " end: " + end);
        while (end > stopwatch.ElapsedMilliseconds)
        {
            //i++;
            //if (i > 10000000)
            //{
            //    break;
            //    Debug.Log("Breaking Elapsed time: " + stopwatch.ElapsedMilliseconds + "ms");
            //}
        }
        stopwatch.Stop();

        //Debug.Log("Completed in: " + stopwatch.ElapsedMilliseconds + "ms");
    }
}
