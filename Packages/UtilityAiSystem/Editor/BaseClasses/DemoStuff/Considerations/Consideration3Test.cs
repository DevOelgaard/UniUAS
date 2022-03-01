using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Consideration3Test : Consideration
{
    protected override float CalculateBaseScore(AiContext context)
    {
        return 0.5f;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}