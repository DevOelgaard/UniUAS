using System;
using System.Collections.Generic;
using UnityEngine;


// https://stackoverflow.com/questions/412019/math-optimization-in-c-sharp
// https://en.wikipedia.org/wiki/Logistic_function
public class LogisticFunction : ResponseFunction
{
    public LogisticFunction() : base("Logistic")
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Max Value", 1f),
            new Parameter("Growth Rate", 10f),
            new Parameter("Midpoint", 0.5f),
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        // L / 1 + e^-k(x-x0)
        return Convert.ToSingle(Parameters[0].Value) / (1.0f + (float)Mathf.Exp(-Convert.ToSingle(Parameters[1].Value) * (x - Convert.ToSingle(Parameters[2].Value))));
    }
}
