using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ConsiderationDebug : AiObjectDebug
{
    internal float BaseScore;
    internal float NormalizedScore;
    internal ParametersDebug Parameters;
    internal ResponseCurveDebug ResponseCurve;

    internal static ConsiderationDebug GetDebug(Consideration consideration)
    {
        var result = new ConsiderationDebug();
        result.Name = consideration.Name;
        result.Description = consideration.Description;
        result.Type = consideration.GetType().ToString();
        result.BaseScore = consideration.BaseScore;
        result.NormalizedScore = consideration.NormalizedScore;
        result.Parameters = new ParametersDebug(consideration.Parameters);
        result.ResponseCurve = ResponseCurveDebug.GetDebug(consideration.CurrentResponseCurve);
        return result;
    }
}
