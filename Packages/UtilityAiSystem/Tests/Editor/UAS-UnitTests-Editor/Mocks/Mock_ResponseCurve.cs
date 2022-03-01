using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mock_ResponseCurve : ResponseCurveModel
{
    public Mock_ResponseCurve(string name, List<Parameter> parameters) : base(name, parameters)
    {
    }

    protected override float ResponseFunction(float x)
    {
        return x;
    }
}
