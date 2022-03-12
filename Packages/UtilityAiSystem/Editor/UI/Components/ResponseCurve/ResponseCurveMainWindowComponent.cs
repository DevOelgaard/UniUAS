using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveMainWindowComponent : MainWindowComponent
{
    public ResponseCurveMainWindowComponent(ResponseCurve mainWindowModel) : base(mainWindowModel)
    {
        var root = new ResponseCurveLCComponent(mainWindowModel, false);
        Body.Clear();
        Body.Add(root);
    }
}
