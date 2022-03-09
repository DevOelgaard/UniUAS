﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

// https://forum.unity.com/threads/draw-a-line-from-a-to-b.698618/
public class ResponseCurveModel: RestoreAble
{
    private Dictionary<Parameter, IDisposable> segmentDisposables = new Dictionary<Parameter, IDisposable>();
    public string Name = "";
    
    private float minY = 0.0f;
    private float maxY = 1.0f;
    private float minX = 0.0f;
    private float maxX = 1.0f;

    public List<ResponseFunction> ResponseFunctions = new List<ResponseFunction>();
    public List<Parameter> Segments = new List<Parameter>();

    public IObservable<bool> OnValuesChanged => onValuesChanged;
    private Subject<bool> onValuesChanged = new Subject<bool>();
    public IObservable<bool> OnSegmentsChanged => onSegmentsChanged;
    private Subject<bool> onSegmentsChanged = new Subject<bool>();

    public ResponseCurveModel()
    {
        var firstFunction = AssetDatabaseService.GetInstancesOfType<ResponseFunction>().First();
        ResponseFunctions = new List<ResponseFunction>();
        AddResponseFunction(firstFunction);
    }

    protected ResponseCurveModel(string name, float minY = 0.0f, float maxY = 1.0f)
    {
        Name = name;
        MinY = minY;
        MaxY = maxY;
        var firstFunction = AssetDatabaseService.GetInstancesOfType<ResponseFunction>().First();
        ResponseFunctions = new List<ResponseFunction>();
        AddResponseFunction(firstFunction);
    }

    public void AddResponseFunction(ResponseFunction newFunction)
    {
        var previouseFunction = ResponseFunctions.LastOrDefault();

        if (previouseFunction == null)
        {
            newFunction.SetMinMaxX(MinX, MaxX, MinX, MaxX);
        } else
        {
            var segmentValue = (float)(previouseFunction.MaxX-previouseFunction.MinX) / 2 + previouseFunction.MinX;
            var newSegment = new Parameter(" ", segmentValue);
            SegmentSubscripe(newSegment, previouseFunction, newFunction);

            previouseFunction.SetMinMaxX(previouseFunction.MinX,segmentValue,MinX,MaxX);
            newFunction.SetMinMaxX(segmentValue,MaxX,MinX,MaxX);

            Segments.Add(newSegment);
        }
        ResponseFunctions.Add(newFunction);
        onValuesChanged.OnNext(true);
    }

    public void RemoveResponseFunction(ResponseFunction responseFunction)
    {
        var functionIndex = ResponseFunctions.IndexOf(responseFunction);
        var removeIndex = functionIndex - 1;
        if (removeIndex < 0) // Removing first or only function
        {
            if (ResponseFunctions.Count <= 0)
            {
                throw new Exception("Can't remove the last Response function");
            }
            ResponseFunctions.Remove(responseFunction);
            RemoveSegment(Segments[0]);
            ResponseFunctions[0].SetMinMaxX(MinX, ResponseFunctions[0].MaxX, MinX, MaxX);
        } 
        else if (functionIndex == Segments.Count) // Removing last function
        {
            ResponseFunctions.Remove(responseFunction);
            RemoveSegment(Segments[removeIndex]);
            var lastFuncion = ResponseFunctions.Last();
            lastFuncion.SetMinMaxX(lastFuncion.MinX, MaxX, MinX, MaxX);
        }
        else
        {
            RemoveSegment(Segments[removeIndex]);
            ResponseFunctions.Remove(responseFunction);

            var segmentToUpdate = Segments[removeIndex];
            //segmentDisposables[segmentToUpdate].Dispose();

            var previousFunction = ResponseFunctions[removeIndex];
            var nextFunction = ResponseFunctions[removeIndex + 1];

            SegmentSubscripe(segmentToUpdate, previousFunction, nextFunction);

            previousFunction.SetMinMaxX(previousFunction.MinX, previousFunction.MaxX, MinX, MaxX);
            nextFunction.SetMinMaxX(previousFunction.MaxX, nextFunction.MaxX, MinX, MaxX);

        }
        onValuesChanged.OnNext(true);
    }

    internal void UpdateFunction(ResponseFunction oldFunction, ResponseFunction newFunction)
    {
        var oldFunctionIndex = ResponseFunctions.IndexOf(oldFunction);
        newFunction.SetMinMaxX(oldFunction.MinX, oldFunction.MaxX, MinX, MaxX);
        ResponseFunctions[oldFunctionIndex] = newFunction;
        UpdateSegmentSubscriptions();

        onValuesChanged.OnNext(true);
    }

    private void SegmentSubscripe(Parameter segment, ResponseFunction previous, ResponseFunction next)
    {
        var segmentSub = segment.OnValueChange
        .Subscribe(value =>
        {
            var valueCast = Convert.ToSingle(value);
            previous.SetMinMaxX(previous.MinX, valueCast, MinX, MaxX);
            next.SetMinMaxX(valueCast, next.MaxX, MinX, MaxX);
        });

        if (segmentDisposables.ContainsKey(segment))
        {
            segmentDisposables[segment].Dispose();
            segmentDisposables[segment] = segmentSub;
        } else
        {
            segmentDisposables.Add(segment, segmentSub);
        }
    }

    private void UpdateSegmentSubscriptions()
    {
        foreach(var segment in Segments)
        {
            //segmentDisposables[segment].Dispose();
            var segmentIndex = Segments.IndexOf(segment);
            var previousFunction = ResponseFunctions[segmentIndex];
            var nextFunction = ResponseFunctions[segmentIndex + 1];

            SegmentSubscripe(segment,previousFunction, nextFunction);
        }
    }

    private void RemoveSegment(Parameter segmentToRemove)
    {
        segmentDisposables[segmentToRemove].Dispose();
        segmentDisposables.Remove(segmentToRemove);
        Segments.Remove(segmentToRemove);
    }

    public float CalculateResponse(float x)
    {
        // Calculate resonse of all functions
        var previousFunctions = new List<ResponseFunction>();
        ResponseFunction lastValidFunction = ResponseFunctions.First();

        for(var i = 0; i < ResponseFunctions.Count; i++)
        {
            var currentFunction = ResponseFunctions[i];
            if (x > currentFunction.MinX)
            {
                lastValidFunction = currentFunction;
            }

            if (ResponseFunctions.Count-1 > i) 
            {
                var nextFunciton = ResponseFunctions[i + 1];
                if (x > nextFunciton.MinX)
                {
                    previousFunctions.Add(lastValidFunction);
                }
            }
        }

        var result = 0f;
        foreach (var function in previousFunctions)
        {
            result += function.GetResponseValue(function.MaxX);
            result = Mathf.Clamp(result, MinY, MaxY);
        }
        if (x > lastValidFunction.MinX)
        {
            result += lastValidFunction.GetResponseValue(x);
        }

        return Mathf.Clamp(result,MinY,MaxY);
    }

    
    protected override void RestoreInternal(RestoreState s)
    {
        var state = (ResponseCurveState)s;
        Name = state.Name;
        MinY = state.MinY;
        MaxY = state.MaxY;
        MinX = state.MinX;
        MaxX = state.MaxX;
        Segments = new List<Parameter>();
        foreach (var p in state.Segments)
        {
            var parameter = Parameter.Restore<Parameter>(p);
            Segments.Add(parameter);
        }
    }
    internal override RestoreState GetState()
    {

        return new ResponseCurveState(Name, MinY, MaxY, Segments, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }

    public float MinY
    {
        get => minY;
        set
        {
            minY = value;
            onValuesChanged.OnNext(true);
        }
    }
    public float MaxY
    {
        get => maxY;
        set
        {
            maxY = value;
            onValuesChanged.OnNext(true);
        }
    }
    public float MinX
    {
        get => minX; 
        set
        {
            var oldRange = MaxX - MinX;
            minX = value;
            UpdateMinMax(oldRange);
            onValuesChanged.OnNext(true);
        }
 
    }
    public float MaxX
    {
        get => maxX;
        set
        {
            var oldRange = MaxX - MinX;
            maxX = value;
            UpdateMinMax(oldRange);
            onValuesChanged.OnNext(true);
        }
    }

    private void UpdateMinMax(float oldRange)
    {
        var factor = (MaxX-MinX) / oldRange;
        foreach (var function in ResponseFunctions)
        {
            function.UpdateValues(factor);
        }
        foreach (var segment in Segments)
        {
            var v = (float)segment.Value;
            segment.Value = v * factor;
        }
    }

    ~ResponseCurveModel()
    {
        foreach(var disposable in segmentDisposables)
        {
            disposable.Value.Dispose();
        }
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
    public List<ParameterState> Segments;

    public ResponseCurveState(): base()
    {
    }

    public ResponseCurveState(string name, float minY, float maxY, List<Parameter> segments, ResponseCurveModel responseCurveModel): base(responseCurveModel)
    {
        throw new NotImplementedException("Needs segments");
        Name = name;
        MinY = minY;
        MaxY = maxY;
        MinX = responseCurveModel.MinX;
        MaxX = responseCurveModel.MaxX;
        Segments = new List<ParameterState>();
        foreach(var parameter in segments)
        {
            Segments.Add(parameter.GetState() as ParameterState);
        }
    }
}