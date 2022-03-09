using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/10097891/inverse-logistic-function-reverse-sigmoid-function
public class RCInverseLogistic : ResponseFunction
{
    public RCInverseLogistic() : base("Inverse Logistic")
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
        };
    }

    protected override float CalculateResponse(float x)
    {
        return Mathf.Log(x / (1 - x));
    }
}
