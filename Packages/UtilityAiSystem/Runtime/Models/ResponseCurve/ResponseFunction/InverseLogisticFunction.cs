﻿using System;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/10097891/inverse-logistic-function-reverse-sigmoid-function
public class InverseLogisticFunction : ResponseFunction
{
    public InverseLogisticFunction() : base("Inverse Logistic")
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Base", 2.8f),
        };
    }

    public override float CalculateResponse(float x)
    {
        var baseLn = Convert.ToSingle(Parameters[0].Value);
        return (((float)Math.Log(x,baseLn) - (float)Math.Log(1-x,baseLn))/10) + 0.5f;
    }
}
