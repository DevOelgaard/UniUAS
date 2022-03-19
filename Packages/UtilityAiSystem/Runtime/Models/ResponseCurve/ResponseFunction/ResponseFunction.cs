using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ResponseFunction: AiObjectModel
{
    public List<Parameter> Parameters;
    private bool Inverse => (bool)Parameters
        .FirstOrDefault(p => p.Name == "Inverse" && p.Value.GetType() == typeof(bool))
        .Value; 

    public ResponseFunction()
    {
        Parameters = GetParameters();
    }

    protected ResponseFunction(string name)
    {
        Name = name;
        Parameters = new List<Parameter>();
        Parameters = GetParameters();
        Parameters.Add(new Parameter("Inverse", false ));
    }

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public float CalculateResponse(float x, float prevResult, float maxY)
    {
        var result = 0f;
        if (Inverse)
        {
            result = 1-CalculateResponseInternal(x);
        } else
        {
            result = CalculateResponseInternal(x);
        }
        result = Normalize(result, prevResult,maxY);
        return result + prevResult;
        //return Normalize(result,minY,maxY);
    }

    private float Normalize(float value, float min, float max)
    {
        var factor = (max - min) / max;
        var x = value * factor;// * ResultFactor;
        return x;
    }

    protected abstract float CalculateResponseInternal(float x);

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (ResponseFunctionState)s;
        Name = state.Name;
        Parameters = new List<Parameter>();
        foreach (var p in state.Parameters)
        {
            var parameter = Parameter.Restore<Parameter>(p, restoreDebug);
            Parameters.Add(parameter);
        }
        if (Parameters.FirstOrDefault(p => p.Name == "Inverse") == null)
        {
            Parameters.Add(new Parameter("Inverse", false));
        }
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = ResponseFunction.Restore<ResponseFunction>(state);
        return clone;
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
    public string Description;
    public List<ParameterState> Parameters;

    public ResponseFunctionState() : base()
    {
    }

    public ResponseFunctionState(ResponseFunction responseFunction) : base(responseFunction)
    {
        Name = responseFunction.Name;
        Description = responseFunction.Description;
        Parameters = new List<ParameterState>();
        foreach (var parameter in responseFunction.Parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }
    }
}