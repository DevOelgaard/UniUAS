using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RCLinear : ResponseCurveModel
{
    public RCLinear() : base("Linear",
        new List<Parameter> {
            new Parameter("a",1f),
            new Parameter("b",0f)
        })
    {
    }

    protected override float ResponseFunction(float x)
    {
        return Convert.ToSingle(Parameters[0].Value) * x + Convert.ToSingle(Parameters[1].Value);
    }
}
