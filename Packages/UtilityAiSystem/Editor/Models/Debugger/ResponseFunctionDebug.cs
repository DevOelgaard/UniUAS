using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ResponseFunctionDebug
{
    public string Type = "";
    public ParametersDebug Parameters;

    internal static ResponseFunctionDebug GetDebug(ResponseFunction rF)
    {
        var result = new ResponseFunctionDebug();
        result.Type = rF.GetType().ToString();
        result.Parameters = new ParametersDebug(rF.Parameters);
        return result;
    }

}