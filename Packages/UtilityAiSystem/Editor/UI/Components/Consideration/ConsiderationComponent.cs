using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine;

internal class ConsiderationComponent : MainWindowComponent
{
    private CompositeDisposable minMaxSubs = new CompositeDisposable();
    private TemplateContainer root;
    private Consideration considerationModel => Model as Consideration;
    private ScoreComponent baseScore => ScoreComponents[0];
    private ScoreComponent normalizedScore => ScoreComponents[1];
    private VisualElement parametersContainer;
    private VisualElement curveContainer;
    private EnumField performanceTag;
    private FloatFieldMinMax minField;
    private FloatFieldMinMax maxField;
    private ResponseCurveLCComponent responseCurveLCComponent;

    internal ConsiderationComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("Parameters");
        curveContainer = root.Q<VisualElement>("Curve");
        performanceTag = root.Q<EnumField>("PerformanceTag");

        Body.Clear();
        Body.Add(root);

        responseCurveLCComponent = new ResponseCurveLCComponent();
        curveContainer.Add(responseCurveLCComponent);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        ClearSubscriptions();
        considerationModel.BaseScoreChanged
            .Subscribe(score => baseScore.UpdateScore(score))
            .AddTo(modelInfoChangedDisposable);

        considerationModel.NormalizedScoreChanged
            .Subscribe(score => normalizedScore.UpdateScore(score))
            .AddTo(modelInfoChangedDisposable);


        performanceTag.Init(PerformanceTag.Normal);
        performanceTag.value = considerationModel.PerformanceTag;
        performanceTag.RegisterCallback<ChangeEvent<Enum>>(evt =>
        {
            considerationModel.PerformanceTag = (PerformanceTag)evt.newValue;
        });

        SetParameters();
        responseCurveLCComponent.UpdateUi(considerationModel.CurrentResponseCurve);
    }

    private void SetParameters()
    {
        Debug.LogWarning("This could be more effective by using a pool");
        parametersContainer.Clear();
        var minParamComp = new ParameterComponent(considerationModel.MinFloat);
        var maxParamComp = new ParameterComponent(considerationModel.MaxFloat);
        parametersContainer.Add(minParamComp);
        parametersContainer.Add(maxParamComp);
        minField = minParamComp.field as FloatFieldMinMax;
        maxField = maxParamComp.field as FloatFieldMinMax;

        minField.Max = Convert.ToSingle(considerationModel.MaxFloat.Value);
        maxField.Min = Convert.ToSingle(considerationModel.MinFloat.Value);

        minMaxSubs.Clear();
        considerationModel.MinFloat
            .OnValueChange
            .Subscribe(value =>
            {
                maxField.Min = Convert.ToSingle(value);
            })
            .AddTo(minMaxSubs);

        considerationModel.MaxFloat
            .OnValueChange
            .Subscribe(value =>
            {
                minField.Max = Convert.ToSingle(value);
            })
            .AddTo(minMaxSubs);

        foreach(var parameter in considerationModel.Parameters)
        {
            parametersContainer.Add(new ParameterComponent(parameter));
        }
    }


    ~ConsiderationComponent()
    {
        minMaxSubs.Clear();
    }
}
