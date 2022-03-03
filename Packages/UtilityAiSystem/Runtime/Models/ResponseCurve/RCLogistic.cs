using UnityEngine;


// https://stackoverflow.com/questions/412019/math-optimization-in-c-sharp
// https://en.wikipedia.org/wiki/Logistic_function
public class RCLogistic : ResponseCurveModel
{
    public RCLogistic() : base("Logistic")
    {
    }

    protected override float ResponseFunction(float x)
    {
        var k = Mathf.Exp(x);
        return k / (1.0f + k);
    }
}
