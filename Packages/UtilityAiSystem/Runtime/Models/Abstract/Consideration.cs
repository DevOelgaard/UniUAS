using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UniRx;
using System.Linq;

public abstract class Consideration : AiObjectModel
{
    private CompositeDisposable paramaterDisposables = new CompositeDisposable();

    private string namePostfix;
    public List<Parameter> Parameters;
    private List<ResponseCurveModel> responseCurves = new List<ResponseCurveModel>();
    private ResponseCurveModel currentResponseCurve;
    public ResponseCurveModel CurrentResponseCurve
    {
        get 
        { 
            if (currentResponseCurve == null)
            {
                currentResponseCurve = AssetDatabaseService.GetInstancesOfType<ResponseCurveModel>()
                    .FirstOrDefault();
            }
            return currentResponseCurve; 
        }
        protected set
        {
            currentResponseCurve = value;
            onResponseCurveChanged.OnNext(currentResponseCurve);
        }
    }
    public IObservable<ResponseCurveModel> OnResponseCurveChanged => onResponseCurveChanged;
    private Subject<ResponseCurveModel> onResponseCurveChanged = new Subject<ResponseCurveModel>();
    public PerformanceTag PerformanceTag;
    public float BaseScore
    {
        get => ScoreModels[0].Value;
        set => ScoreModels[0].Value = value;
    }
    public IObservable<float> BaseScoreChanged => ScoreModels[0].OnValueChanged;
    public float NormalizedScore
    {
        get => ScoreModels[1].Value;
        set => ScoreModels[1].Value = value;
    }
    public IObservable<float> NormalizedScoreChanged => ScoreModels[1].OnValueChanged;

    public Parameter Min = new Parameter("Min", 0f);
    public Parameter Max = new Parameter("Max", 1f);

    protected Consideration()
    {
        Parameters =  new List<Parameter>(GetParameters());
        ScoreModels = new List<ScoreModel>();
        ScoreModels.Add(new ScoreModel("Base", 0f));
        ScoreModels.Add(new ScoreModel("Normalized", 0f));
        namePostfix = " (" + TypeDescriptor.GetClassName(this) + ")";
        PerformanceTag = GetPerformanceTag();
        responseCurves = AssetDatabaseService.GetInstancesOfType<ResponseCurveModel>();
        
        CurrentResponseCurve = responseCurves.FirstOrDefault();

        Min.OnValueChange
            .Subscribe(_ => SetMinMaxForCurves())
            .AddTo(paramaterDisposables);

        Max.OnValueChange
            .Subscribe(_ => SetMinMaxForCurves())
            .AddTo(paramaterDisposables);

        SetMinMaxForCurves();
    }

    private void SetMinMaxForCurves()
    {
        foreach(var curve in responseCurves)
        {
            curve.MinX = Convert.ToSingle(Min.Value);
            curve.MaxX = Convert.ToSingle(Max.Value);
        }
        CurrentResponseCurve.MinX = Convert.ToSingle(Min.Value);
        CurrentResponseCurve.MaxX = Convert.ToSingle(Max.Value);
    }

    public override string GetNameFormat(string name)
    {
        if (!name.Contains(namePostfix))
        {
            return name + namePostfix;
        }
        return name;
    }

    protected virtual PerformanceTag GetPerformanceTag()
    {
        return PerformanceTag.Normal;
    }

    protected abstract List<Parameter> GetParameters();
    protected abstract float CalculateBaseScore(AiContext context);

    internal void SetResponseCurve(Type t)
    {
        var newValue = responseCurves.FirstOrDefault(rC => rC.GetType() == t);
        if (newValue == null) return;
        CurrentResponseCurve = newValue;
    }

    internal void SetResponseCurve(string name)
    {
        var newValue = responseCurves.FirstOrDefault(rC => rC.Name == name);
        if (newValue == null) return;
        CurrentResponseCurve = newValue;
    }

    public virtual float CalculateScore(AiContext context)
    {
        BaseScore = CalculateBaseScore(context);
        if (BaseScore < Convert.ToSingle(Min.Value))
        {
            return BaseScoreBelowMinValue();
        }
        else if (BaseScore > Convert.ToSingle(Max.Value))
        {
            return BaseScoreAboveMaxValue();
        }
        //var normalizedBaseScore = Normalize(BaseScore);
        var response = CurrentResponseCurve.CalculateResponse(BaseScore);
        NormalizedScore = Mathf.Clamp(response, 0f, 1f);

        return NormalizedScore;
    }



    protected virtual float BaseScoreBelowMinValue()
    {
        return 0;
    }

    protected virtual float BaseScoreAboveMaxValue()
    {
        return 1;
    }

    protected override void RestoreInternal(RestoreState s)
    {
        var state = (ConsiderationState)s;
        Name = state.Name;
        Description = state.Description;
        Min = Parameter.Restore<Parameter>(state.Min);
        Max = Parameter.Restore<Parameter>(state.Max);

        CurrentResponseCurve = ResponseCurveModel.Restore<ResponseCurveModel>(state.ResponseCurveState);

        Parameters = new List<Parameter>();
        foreach (var pState in state.Parameters)
        {
            var parameter = Parameter.Restore<Parameter>(pState);
            Parameters.Add(parameter);
        }
        PerformanceTag = (PerformanceTag)state.PerformanceTag;

        if (state.ResponseCurveStates != null)
        {
            responseCurves = new List<ResponseCurveModel>();
            foreach (var rC in state.ResponseCurveStates)
            {
                responseCurves.Add(Restore<ResponseCurveModel>(rC));
            }
        } else
        {
            responseCurves = AssetDatabaseService.GetInstancesOfType<ResponseCurveModel>();
        }

        paramaterDisposables.Clear();
        Min.OnValueChange
            .Subscribe(_ => SetMinMaxForCurves())
            .AddTo(paramaterDisposables);

        Max.OnValueChange
            .Subscribe(_ => SetMinMaxForCurves())
            .AddTo(paramaterDisposables);

        SetMinMaxForCurves();
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Consideration>(state);
        return clone;
    }

    internal override RestoreState GetState()
    {
        return new ConsiderationState(Name,Description,Parameters, CurrentResponseCurve, Min, Max, responseCurves, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }


    ~Consideration()
    {
        paramaterDisposables.Clear();
    }
}

[Serializable]
public class ConsiderationState: RestoreState
{
    public List<ParameterState> Parameters;
    public ResponseCurveState ResponseCurveState;
    public string Name;
    public string Description;
    public ParameterState Min;
    public ParameterState Max;
    public int PerformanceTag;
    public List<ResponseCurveState> ResponseCurveStates;

    public ConsiderationState() : base()
    {
    }

    public ConsiderationState(string name, string description, List<Parameter> parameters, ResponseCurveModel responseCurve, Parameter min, Parameter max, List<ResponseCurveModel> responseCurves, Consideration consideration): base(consideration)
    {
        Name = name;
        Description = description;
        ResponseCurveState = responseCurve.GetState() as ResponseCurveState;
        Min = min.GetState() as ParameterState;
        Max = max.GetState() as ParameterState;

        Parameters = new List<ParameterState>();
        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }

        PerformanceTag = (int)consideration.PerformanceTag;

        ResponseCurveStates = new List<ResponseCurveState>();
        foreach(var rC in responseCurves)
        {
            ResponseCurveStates.Add(rC.GetState() as ResponseCurveState);
        }
    }
}