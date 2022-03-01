using UnityEngine;

// https://stackoverflow.com/questions/10097891/inverse-logistic-function-reverse-sigmoid-function
public class RCInverseLogistic : ResponseCurveModel
{
    public RCInverseLogistic() : base("Inverse Logistic")
    {
    }

    protected override float ResponseFunction(float x)
    {
        return Mathf.Log(x / (1 - x));
    }
}
