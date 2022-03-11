using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;


internal class ResponseCurveDebugComponent : AiObjectDebugComponent
{
    public ResponseCurveDebugComponent(ResponseCurveDebug rc) : base(rc)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        foreach(var rf in rc.ResponseFunctions)
        {
            Body.Add(new ResponseFunctionDebugComponent(rf));
        }
    }
}