using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsideration
{
    public float CalculateScore(AiContext context);
    protected float CalculateBaseScore(AiContext context);
    protected abstract List<Parameter> GetParameters();

}
