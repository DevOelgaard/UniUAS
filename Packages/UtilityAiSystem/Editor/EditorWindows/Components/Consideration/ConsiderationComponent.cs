using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;

internal class ConsiderationComponent : MainWindowComponent
{
    private TemplateContainer root;
    private Consideration considerationModel => Model as Consideration;
    private ScoreComponent baseScore => ScoreComponents[0];
    private ScoreComponent normalizedScore => ScoreComponents[1];
    private VisualElement parametersContainer;
    private VisualElement curveContainer;

    internal ConsiderationComponent(Consideration considerationModel) : base(considerationModel)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("Parameters");
        curveContainer = root.Q<VisualElement>("Curve");

        Body.Clear();
        Body.Add(root);

        ClearSubscriptions();
        considerationModel.BaseScoreChanged
            .Subscribe(score => baseScore.UpdateScore(score))
            .AddTo(Disposables);

        considerationModel.NormalizedScoreChanged
            .Subscribe(score => normalizedScore.UpdateScore(score))
            .AddTo(Disposables);

        SetParameters();
        curveContainer.Add(new ResponseCurveComponent(considerationModel));
        
    }

    private void SetParameters()
    {
        parametersContainer.Clear();
        parametersContainer.Add(new ParameterComponent(considerationModel.Min));
        parametersContainer.Add(new ParameterComponent(considerationModel.Max));

        foreach(var parameter in considerationModel.Parameters)
        {
            parametersContainer.Add(new ParameterComponent(parameter));
        }
    }
}
