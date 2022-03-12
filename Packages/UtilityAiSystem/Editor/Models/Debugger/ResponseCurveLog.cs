using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveLog: AiObjectLog
{
    public List<ResponseFunctionLog> ResponseFunctions;

    internal static ResponseCurveLog GetDebug(ResponseCurve responseCurve)
    {
        var result = new ResponseCurveLog();
        result.Name = responseCurve.Name;
        result.Description = responseCurve.Description;
        result.Type = responseCurve.GetType().ToString();
        result.ResponseFunctions = new List<ResponseFunctionLog>();
        foreach(var rF in responseCurve.ResponseFunctions)
        {
            result.ResponseFunctions.Add(ResponseFunctionLog.GetDebug(rF));
        }

        return result;
    }
}