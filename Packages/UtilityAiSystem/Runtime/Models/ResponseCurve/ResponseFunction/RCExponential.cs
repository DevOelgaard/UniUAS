using System;
using System.Collections.Generic;
using UnityEngine;


public class RCExponential : ResponseFunction
{
    public RCExponential() : base("Exponential") {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter> {
            new Parameter("Power",2f),
            new Parameter("b",0f)
            };
    }

    protected override float CalculateResponse(float x)
    {
        return Mathf.Pow(x, Convert.ToSingle(Parameters[0].Value)) + Convert.ToSingle(Parameters[1].Value);
    }
}
