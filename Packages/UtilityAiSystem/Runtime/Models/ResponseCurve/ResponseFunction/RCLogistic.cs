using System.Collections.Generic;
using UnityEngine;


// https://stackoverflow.com/questions/412019/math-optimization-in-c-sharp
// https://en.wikipedia.org/wiki/Logistic_function
public class RCLogistic : ResponseFunction
{
    public RCLogistic() : base("Logistic")
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    protected override float CalculateResponse(float x)
    {
        var k = Mathf.Exp(x);
        return k / (1.0f + k);
    }
}
