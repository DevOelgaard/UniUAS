using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;
using UnityEditor.UIElements;

internal class ResponseCurveLCComponent : VisualElement
{
    private CompositeDisposable parameterDisposables = new CompositeDisposable();
    private CompositeDisposable considerationDisposables = new CompositeDisposable();
    private Consideration consideration;
    private ResponseCurveModel responseCurve;
    private LineChartComponent lineChart;

    private float min => Convert.ToSingle(consideration.Min.Value);
    private float max => Convert.ToSingle(consideration.Max.Value);
    private int steps = 100;

    //private Label nameLabel;
    private DropdownField responseCurveDropdown;
    private VisualElement body;
    private VisualElement footer;
    private IntegerField resolution;
    public ResponseCurveLCComponent(Consideration consideration)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        //nameLabel = root.Q<Label>("ChartName-Label");
        responseCurveDropdown = root.Q<DropdownField>("ResponseCurve-Dropdown");
        body = root.Q<VisualElement>("Body");
        footer = root.Q<VisualElement>("Footer");

        this.consideration = consideration;
        //this.responseCurve = consideration.CurrentResponseCurve;
        lineChart = new LineChartComponent();
        body.Add(lineChart);

        responseCurveDropdown.value = consideration.CurrentResponseCurve.Name;
        responseCurveDropdown.choices = ResponseCurveService.GetResponseCurveNames();
        responseCurveDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            consideration.SetResponseCurve(evt.newValue);
        });

        resolution = new IntegerField("Resolution");
        resolution.value = steps;
        resolution.RegisterCallback<ChangeEvent<int>>(evt =>
        {
            steps = evt.newValue;
            ReDrawChart();
        });

        consideration.OnResponseCurveChanged
            .Subscribe(_ =>
            {
                UpdateUi();
                ReDrawChart();
            })
            .AddTo(considerationDisposables);

        consideration.Min
            .OnValueChange
            .Subscribe(_ =>
            {
                ReDrawChart();
            })
            .AddTo(considerationDisposables);

        consideration.Max
            .OnValueChange
            .Subscribe(_ =>
            {
                ReDrawChart();
            })
            .AddTo(considerationDisposables);

        UpdateUi();
        ReDrawChart();
    }

    private void UpdateUi()
    {
        if (responseCurve != consideration.CurrentResponseCurve)
        {
            responseCurve = consideration.CurrentResponseCurve;

            //nameLabel.text = responseCurve.Name;
            footer.Clear();
            footer.Add(resolution);
            foreach(var param in responseCurve.Parameters)
            {
                footer.Add(new ParameterComponent(param));
                param
                    .OnValueChange
                    .Subscribe(_ =>
                    {
                        ReDrawChart();
                    })
                    .AddTo(parameterDisposables);
            }
        }
    }

    private void ReDrawChart()
    {
        var points = new List<Vector2>();
        var stepSize = (max - min) / steps;
        for (var i = 0; i <= steps; i++)
        {
            var x = i * stepSize + min;
            var y = consideration.CurrentResponseCurve.CalculateResponse(x);
            points.Add(new Vector2(x, y));
        }

        lineChart?.DrawCurve(points, min, max);
    }

    ~ResponseCurveLCComponent(){
        parameterDisposables.Clear();
        considerationDisposables.Clear();
    }
}