using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TestConsideration : Consideration
{
    protected override float CalculateBaseScore(AiContext context)
    {
        return 0.81f;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>
        {
            new Parameter("T-Float", 0.5f),
            new Parameter("T-String", "string value"),
            new Parameter("T-Int", 11),
        };
    }
}
