using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;


internal class ResponseCurveLogComponent : AiObjectLogComponent
{
    //private ResponseFunctionLogPool pool;
    private LogComponentPool<ResponseFunctionLogComponent> pool;
    public ResponseCurveLogComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(this.GetType().FullName);
        Body.Add(root);
        pool = new LogComponentPool<ResponseFunctionLogComponent>(root);
        
    }

    protected override void DisplayInternal(AiObjectLog aiObjectDebug)
    {
        var rc = aiObjectDebug as ResponseCurveLog;
        var logModels = new List<ILogModel>();
        foreach(var rf in rc.ResponseFunctions)
        {
            logModels.Add(rf);
        }
        pool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        pool.Hide();
    }
}