﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RCLinear : ResponseFunction
{
    public RCLinear() : base("Linear")
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("a",1f),
            new Parameter("b",0f)
        };
    }

    protected override float CalculateResponse(float x)
    {
        return Convert.ToSingle(Parameters[0].Value) * x + Convert.ToSingle(Parameters[1].Value);
    }
}
