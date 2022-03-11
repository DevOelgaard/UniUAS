using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveDebug: AiObjectDebug
{
    public List<ResponseFunctionDebug> ResponseFunctions;

    internal static ResponseCurveDebug GetDebug(ResponseCurve responseCurve)
    {
        var result = new ResponseCurveDebug();
        result.Name = responseCurve.Name;
        result.Description = responseCurve.Description;
        result.Type = responseCurve.GetType().ToString();
        result.ResponseFunctions = new List<ResponseFunctionDebug>();
        foreach(var rF in responseCurve.ResponseFunctions)
        {
            result.ResponseFunctions.Add(ResponseFunctionDebug.GetDebug(rF));
        }

        return result;
    }
}