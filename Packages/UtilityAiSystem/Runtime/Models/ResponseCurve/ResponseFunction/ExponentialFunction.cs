﻿using System;
using System.Collections.Generic;
using UnityEngine;


public class ExponentialFunction : ResponseFunction
{
    public ExponentialFunction() : base("Exponential") {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter> {
            new Parameter("Power",2f),
            };
    }

    public override float CalculateResponse(float x)
    {
        return Mathf.Pow(x, Convert.ToSingle(Parameters[0].Value));
    }
}