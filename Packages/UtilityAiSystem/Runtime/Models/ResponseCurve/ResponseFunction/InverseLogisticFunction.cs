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
            new Parameter("Max Value", 1f),
            new Parameter("Growth Rate", 10f),
            new Parameter("Midpoint", 0.5f),
        };
    }

    public override float CalculateResponse(float x)
    {
        // L / 1 + e^-k(x-x0)
        return Mathf.Log(x / (1 - x));
    }
}
