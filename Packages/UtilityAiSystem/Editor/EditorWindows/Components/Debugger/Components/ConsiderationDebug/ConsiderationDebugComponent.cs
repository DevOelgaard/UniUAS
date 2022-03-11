using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ConsiderationDebugComponent : AiObjectDebugComponent
{
    private VisualElement parametersContianer;
    private VisualElement responseCurveContainer;
    public ConsiderationDebugComponent(ConsiderationDebug c): base(c)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        parametersContianer = root.Q<VisualElement>("ParametersContainer");
        responseCurveContainer = root.Q<VisualElement>("ResponseCurveContainer");
        var baseScore = new ScoreComponent(new ScoreModel("BaseScore", c.BaseScore));
        ScoreContainer.Add(baseScore);
        var normalizedScore = new ScoreComponent(new ScoreModel("Normalized", c.NormalizedScore));
        ScoreContainer.Add(normalizedScore);

        foreach(var p in c.Parameters.Parameters)
        {
            parametersContianer.Add(new ParameterDebugComponent(p));
        }
        responseCurveContainer.Add(new ResponseCurveDebugComponent(c.ResponseCurve));
    }
}
