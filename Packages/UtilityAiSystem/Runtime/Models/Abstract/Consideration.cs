using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.ComponentModel;

public abstract class Consideration : AiObjectModel
{
    private string namePostfix;
    public List<Parameter> Parameters;
    public ResponseCurveModel ResponseCurve = new RCLinear();
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

    internal void SetResponseCurve(ResponseCurveModel responseCurve)
    {
        ResponseCurve = responseCurve;
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
        var normalizedBaseScore = Normalize(BaseScore);
        var response = ResponseCurve.CalculateResponse(normalizedBaseScore);
        NormalizedScore = Mathf.Clamp(response, 0f, 1f);

        return NormalizedScore;
    }

    private float Normalize(float value)
    {
        var x = (value - Convert.ToSingle(Min.Value)) / (Convert.ToSingle(Max.Value) - Convert.ToSingle(Min.Value));
        return Mathf.Clamp(x, 0, 1);
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

        ResponseCurve = ResponseCurveModel.Restore<ResponseCurveModel>(state.ResponseCurveState);

        Parameters = new List<Parameter>();
        foreach (var pState in state.Parameters)
        {
            var parameter = Parameter.Restore<Parameter>(pState);
            Parameters.Add(parameter);
        }
        PerformanceTag = (PerformanceTag)state.PerformanceTag;
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Consideration>(state);
        return clone;
    }

    internal override RestoreState GetState()
    {
        return new ConsiderationState(Name,Description,Parameters, ResponseCurve, Min, Max, this);
    }

    //internal void Restore(ConsiderationState state)
    //{

    //}

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
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

    public ConsiderationState() : base()
    {
    }

    public ConsiderationState(string name, string description, List<Parameter> parameters, ResponseCurveModel responseCurve, Parameter min, Parameter max, Consideration consideration): base(consideration)
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
    }
}