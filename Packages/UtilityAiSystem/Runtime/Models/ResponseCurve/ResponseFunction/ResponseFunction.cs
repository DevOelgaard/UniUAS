using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ResponseFunction: RestoreAble
{
    public string Name = "";
    public float MinX { get; private set; } = 0.0f;
    public float MaxX { get; private set; } = 1.0f;
    public float MinY { get; private set; } = 0.0f;
    private float totalMax;
    public float ResultFactor { get; private set; } = 1.0f;
    public List<Parameter> Parameters;

    public ResponseFunction()
    {
        Parameters = GetParameters();
    }

    protected ResponseFunction(string name)
    {
        Name = name;
        Parameters = GetParameters();
    }

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public void SetMinMaxX(float localMinX, float localMaxX, float totalMinX , float totalMaxX)
    {
        var totalRange = totalMaxX - totalMinX;
        MinX = localMinX;
        MinY = localMinX / totalRange;
        MaxX = localMaxX;
        ResultFactor = (MaxX-MinX) / totalMaxX;
    }

    public void UpdateValues(float maxFactor)
    {
        MinX *= maxFactor;
        MaxX *= maxFactor;
    }

    protected abstract float CalculateResponse(float x);
    public float GetResponseValue(float x)
    {
        var normalized = Normalize(x);
        return CalculateResponse(normalized);
    }

    private float Normalize(float value)
    {
        var x = (value - MinX) / (MaxX - MinX) * ResultFactor;
        //var result = Mathf.Clamp(x, 0, 1);
        return x;
       
    }
    protected override void RestoreInternal(RestoreState s)
    {
        var state = (ResponseFunctionState)s;
        Name = state.Name;
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
        return new ResponseFunctionState(this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
}

[Serializable]
public class ResponseFunctionState : RestoreState
{
    public string Name;
    public float MinX;
    public float MaxX;
    public List<ParameterState> Parameters;

    public ResponseFunctionState() : base()
    {
    }

    public ResponseFunctionState(ResponseFunction responseFunction) : base(responseFunction)
    {
        Name = responseFunction.Name;
        MinX = responseFunction.MinX;
        MaxX = responseFunction.MaxX;
        Parameters = new List<ParameterState>();
        foreach (var parameter in responseFunction.Parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }
    }
}