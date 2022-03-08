using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using UniRx;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;

public class ResponseCurveComponentOLD: VisualElement
{
    private CompositeDisposable considerationModelSubscriptions = new CompositeDisposable();
    private CompositeDisposable responseCurveSubscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private Consideration considerationModel;
    private DropdownField curveTypeDropdown;
    private VisualElement parametersContainer;
    private VisualElement curveContainer;
    private LineChartComponent lineChartComponent;
    private float min => Convert.ToSingle(considerationModel.Min.Value);
    private float max => Convert.ToSingle(considerationModel.Max.Value);
    private int steps = 100;

    public ResponseCurveComponentOLD(Consideration considerationModel)
    {
        var path = AssetDatabaseService.GetAssetPath(GetType().FullName, "uxml");
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        var x = template.CloneTree();
        Add(x);

        this.considerationModel = considerationModel;
        curveTypeDropdown = this.Q<DropdownField>("CurveType");
        parametersContainer = this.Q<VisualElement>("Parameters");
        curveContainer = this.Q<VisualElement>("CurveContainer");

        curveTypeDropdown.choices = ResponseCurveService.GetResponseCurveNames();
        curveTypeDropdown.value = considerationModel.CurrentResponseCurve.Name;
        curveTypeDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            considerationModel.SetResponseCurve(evt.newValue)
        );

        //lineChartComponent = new LineChartComponent();
        //curveContainer.Add(lineChartComponent);


        considerationModel.Min
            .OnValueChange
            .Subscribe(_ => DrawCurve())
            .AddTo(considerationModelSubscriptions);


        considerationModel.Max
            .OnValueChange
            .Subscribe(_ => DrawCurve())
            .AddTo(considerationModelSubscriptions);

        considerationModel
            .OnResponseCurveChanged
            .Subscribe(_ => UpdateResponseCurve())
            .AddTo(considerationModelSubscriptions);
    }


    public void UpdateResponseCurve()
    {
        responseCurveSubscriptions.Clear();
        parametersContainer.Clear();
        foreach(var parameter in considerationModel.CurrentResponseCurve.Parameters)
        {
            var pComponent = new ParameterComponent(parameter);
            parametersContainer.Add(pComponent);

            parameter.OnValueChange
                .Subscribe(v =>
                {
                    DrawCurve();
                })
                .AddTo(responseCurveSubscriptions);
        }
    }

    private void DrawCurve()
    {
        var points = new List<Vector2>();
        var stepSize = (max - min) / steps;
        for(var i = 0; i <= steps; i++)
        {
            var x = i * stepSize + min;
            var y = considerationModel.CurrentResponseCurve.CalculateResponse(x);
            points.Add(new Vector2(x, y));
        }

        lineChartComponent?.DrawCurve(points, min, max);
    }

    ~ResponseCurveComponentOLD()
    {
        considerationModelSubscriptions.Clear();
        responseCurveSubscriptions.Clear();
    }
}