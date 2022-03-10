using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FixedValueFunction : ResponseFunction
{
    public FixedValueFunction() : base("Fixed Value")
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Return", 1f),
        };
    }

    public override float CalculateResponse(float x)
    {
        return Convert.ToSingle(Parameters[0].Value);
    }
}