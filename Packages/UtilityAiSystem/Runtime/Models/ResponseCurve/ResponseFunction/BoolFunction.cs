using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BoolFunction : ResponseFunction
{
    public BoolFunction() : base(TypeToName.RF_Bool)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("First Value", true),
            new Parameter("CutOff", 0.5f),
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        if (x < Convert.ToSingle(Parameters[1].Value)){
            return (bool)Parameters[0].Value == true ? 1f : 0f;
        } else
        {
            return (bool)Parameters[0].Value == true ? 0f : 1f;
        }
    }
}