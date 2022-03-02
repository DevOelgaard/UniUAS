using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_ConsiderationRandomValue : Consideration
{
    protected override float CalculateBaseScore(AiContext context)
    {
        return UnityEngine.Random.Range(Convert.ToSingle(Min.Value), Convert.ToSingle(Max.Value));
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
