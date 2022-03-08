﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// https://forum.unity.com/threads/draw-a-line-from-a-to-b.698618/
public abstract class ResponseCurveModel: RestoreAble
{
    public string Name = "";
    public float MinY = 0.0f;
    public float MaxY = 1.0f;
    public float MinX = 0.0f;
    public float MaxX = 1.0f;

    public List<Parameter> Parameters = new List<Parameter>();

    protected ResponseCurveModel(string name, List<Parameter> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    protected ResponseCurveModel(string name, float minY = 0.0f, float maxY = 1.0f)
    {
        Name = name;
        MinY = minY;
        MaxY = maxY;
        Parameters = GetParameters();
    }

    public float CalculateResponse(float x)
    {
        var normalizedX = Normalize(x);
        var response = ResponseFunction(normalizedX);
        //var result = response * MaxY + MinY;
        return Mathf.Clamp(response, MinY, MaxY);
    }

    private float Normalize(float value)
    {
        var x = (value - MinX) / (MaxX - MinX);
        return Mathf.Clamp(x, 0, 1);
    }
    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    protected abstract float ResponseFunction(float x);

    protected override void RestoreInternal(RestoreState s)
    {
        var state = (ResponseCurveState)s;
        Name = state.Name;
        MinY = state.MinY;
        MaxY = state.MaxY;
        MinX = state.MinX;
        MaxX = state.MaxX;
        Parameters = new List<Parameter>();
        foreach (var p in state.Parameters)
        {
            var parameter = Parameter.Restore<Parameter>(p);
            Parameters.Add(parameter);
        }
    }
    internal override RestoreState GetState()
    {
        return new ResponseCurveState(Name, MinY, MaxY, Parameters, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
}

[Serializable]
public class ResponseCurveState: RestoreState
{
    public string Name;
    public float MinY;
    public float MaxY;
    public float MinX;
    public float MaxX;
    public List<ParameterState> Parameters;

    public ResponseCurveState(): base()
    {
    }

    public ResponseCurveState(string name, float minY, float maxY, List<Parameter> parameters, ResponseCurveModel responseCurveModel): base(responseCurveModel)
    {
        Name = name;
        MinY = minY;
        MaxY = maxY;
        MinX = responseCurveModel.MinX;
        MaxX = responseCurveModel.MaxX;
        Parameters = new List<ParameterState>();
        foreach(var parameter in parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }
    }
}