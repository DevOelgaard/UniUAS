using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mock_ResponseCurve_IT:ResponseCurveModel
{
    public Mock_ResponseCurve_IT(string name, List<Parameter> parameters) : base(name, parameters)
    {
    }

    protected override float ResponseFunction(float x)
    {
        return x;
    }
}
