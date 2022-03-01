using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ConsiderationTester2 : Consideration
{
    protected override float CalculateBaseScore(AiContext context)
    {
        return 1f;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }


}
