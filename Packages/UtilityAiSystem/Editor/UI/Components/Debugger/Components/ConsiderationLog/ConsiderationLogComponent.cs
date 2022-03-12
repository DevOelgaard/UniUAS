using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ConsiderationLogComponent : AiObjectLogComponent
{
    private VisualElement parametersContianer;
    private VisualElement responseCurveContainer;
    private ScoreComponent baseScore;
    private ScoreComponent normalizedScore;
    private ResponseCurveLogComponent responseCurve;
    private LogComponentPool<ParameterLogComponent> parameterPool;
    public ConsiderationLogComponent(): base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        parametersContianer = root.Q<VisualElement>("ParametersContainer");
        responseCurveContainer = root.Q<VisualElement>("ResponseCurveContainer");
        baseScore = new ScoreComponent(new ScoreModel("BaseScore", 0f));
        ScoreContainer.Add(baseScore);
        normalizedScore = new ScoreComponent(new ScoreModel("Normalized", 0f));
        ScoreContainer.Add(normalizedScore);

        responseCurve = new ResponseCurveLogComponent();
        responseCurveContainer.Add(responseCurve);

        parameterPool = new LogComponentPool<ParameterLogComponent>(parametersContianer,1);
    }

    protected override void DisplayInternal(AiObjectLog aiObjectDebug)
    {
        var c = aiObjectDebug as ConsiderationLog;

        var logModels = new List<ILogModel>();
        foreach(var p in c.Parameters)
        {
            logModels.Add(p);
        }
        parameterPool.Display(logModels);

        baseScore.UpdateScore(c.BaseScore);
        normalizedScore.UpdateScore(c.NormalizedScore);
        responseCurve.Display(c.ResponseCurve);
    }

    internal override void Hide()
    {
        base.Hide();
        parameterPool.Hide();
    }
}
