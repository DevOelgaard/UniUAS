using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ConsiderationLog : AiObjectLog
{
    internal float BaseScore;
    internal float NormalizedScore;
    internal List<ParameterLog> Parameters;
    internal ResponseCurveLog ResponseCurve;

    internal static ConsiderationLog GetDebug(Consideration consideration)
    {
        var result = new ConsiderationLog();
        result.Name = consideration.Name;
        result.Description = consideration.Description;
        result.Type = consideration.GetType().ToString();
        result.BaseScore = consideration.BaseScore;
        result.NormalizedScore = consideration.NormalizedScore;
        result.Parameters = new List<ParameterLog>();
        foreach(var p in consideration.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p));
        }

        result.ResponseCurve = ResponseCurveLog.GetDebug(consideration.CurrentResponseCurve);
        return result;
    }
}
