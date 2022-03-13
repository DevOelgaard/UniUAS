using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveMainWindowComponent : MainWindowComponent
{
    private ResponseCurveLCComponent responseCurveLCComponent;
    public ResponseCurveMainWindowComponent() : base()
    {
        responseCurveLCComponent = new ResponseCurveLCComponent();
        Body.Clear();
        Body.Add(responseCurveLCComponent);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var m = model as ResponseCurve;
        responseCurveLCComponent.UpdateUi(m);
    }
}
