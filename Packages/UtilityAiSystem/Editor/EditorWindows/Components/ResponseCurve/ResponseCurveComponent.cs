using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using UniRx;
using UnityEditor.Experimental.GraphView;
using System;

public class ResponseCurveComponent: VisualElement
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private Consideration considerationModel;
    private DropdownField curveTypeDropdown;
    private VisualElement parametersContainer;
    private VisualElement curveContainer;
    private CurveField curveField;

    private static int steps = 100;

    public ResponseCurveComponent(Consideration considerationModel)
    {
        var path = AssetDatabaseService.GetAssetPath(GetType().FullName, "uxml");
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        var x = template.CloneTree();
        Add(x);

        this.considerationModel = considerationModel;
        curveTypeDropdown = this.Q<DropdownField>("CurveType");
        curveTypeDropdown.choices = ResponseCurveService.GetResponseCurveNames();
        curveTypeDropdown.value = considerationModel.ResponseCurve.Name;
        curveTypeDropdown.RegisterCallback<ChangeEvent<string>>(evt => CurveTypeChanged(evt.newValue));

        parametersContainer = this.Q<VisualElement>("Parameters");
        curveContainer = this.Q<VisualElement>("CurveContainer");
        SetCurveField(considerationModel.ResponseCurve.Name);

        UpdateResponseCurve(considerationModel.ResponseCurve);

    }



    public void UpdateResponseCurve(ResponseCurveModel responseCurve)
    {
        considerationModel.ResponseCurve = responseCurve;

        SetCurveField(considerationModel.ResponseCurve.Name);

        ReDrawCurve();

        subscriptions.Clear();
        parametersContainer.Clear();
        foreach(var parameter in responseCurve.Parameters)
        {
            var pComponent = new ParameterComponent(parameter);
            parametersContainer.Add(pComponent);

            parameter.OnValueChange
                .Subscribe(v =>
                {
                    ReDrawCurve();
                })
                .AddTo(subscriptions);
        }

        considerationModel.Min.OnValueChange
            .Subscribe(_ => ReDrawCurve())
            .AddTo(subscriptions);

        considerationModel.Max.OnValueChange
            .Subscribe(_ => ReDrawCurve())
            .AddTo(subscriptions);
    }

    private void SetCurveField(string name)
    {
        curveContainer.Clear();
        curveField = new CurveField(name);

        curveContainer.Add(curveField);

        var linecChart = new LineChartComponent();
        curveContainer.Add(linecChart);
    }

    private void ReDrawCurve()
    {
        var type = considerationModel.Min.Value.GetType();
        var minX = Convert.ToSingle(considerationModel.Min.Value);
        var maxX = Convert.ToSingle(considerationModel.Max.Value);
        curveField.ranges = new Rect(minX, 0, maxX, 1);
        var stepSize = (maxX - minX) / steps;

        var keyframes = new Keyframe[steps + 1];

        var x = minX;
        for (var i = 0; i < steps + 1; i++)
        {
            var keyframe = new Keyframe(x, considerationModel.ResponseCurve.CalculateResponse(x));
            keyframes[i] = keyframe;
            x += stepSize;
        }

        var points = new AnimationCurve(keyframes);
        curveField.value = points;
        curveContainer.Clear();
        curveContainer.Add(curveField);
        var linecChart = new LineChartComponent();
        curveContainer.Add(linecChart);
    }

    private void CurveTypeChanged(string name)
    {
        var responseCurve = ResponseCurveService.GetResponseCurve(name);
        UpdateResponseCurve(responseCurve);
    }

    ~ResponseCurveComponent()
    {
        subscriptions.Clear();
    }
}