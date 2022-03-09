using System;
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
        var lastFunction = ResponseFunctions.LastOrDefault();

        if (lastFunction == null)
        {
            newFunction.SetMinMaxX(MinX, MaxX, MinX, MaxX);
            //newFunction.SetMinX(MinX, MaxX - MinX);
            //newFunction.SetMaxX(MaxX, MaxX);
        } else
        {
            var segmentValue = (float)lastFunction.MaxX / 2;
            var newSegment = new Parameter(" ", segmentValue);
            var segmentSub = newSegment.OnValueChange
                .Subscribe(value =>
                {
                    var valueCast = Convert.ToSingle(value);
                    lastFunction.SetMinMaxX(lastFunction.MinX,valueCast,MinX,MaxX);
                    newFunction.SetMinMaxX(valueCast,newFunction.MaxY,MinX,MaxX);
                    //lastFunction.SetMaxX(valueCast, MaxX);
                    //newFunction.SetMinX(valueCast, MaxX - MinX);
                });

            segmentDisposables.Add(newSegment,segmentSub);

            lastFunction.SetMinMaxX(lastFunction.MinX,segmentValue,MinX,MaxX);
            newFunction.SetMinMaxX(segmentValue,newFunction.MaxY,MinX,MaxX);
            //lastFunction.SetMaxX(segmentValue, MaxX);
            //newFunction.SetMinX(segmentValue, MaxX - MinX);

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
            ResponseFunctions.Remove(responseFunction);
            if (ResponseFunctions.Count <= 0)
            {
                return;
            }
            RemoveSegment(Segments[0]);
            //ResponseFunctions[0].SetMinX(MinX,MaxX-MinX);
            ResponseFunctions[0].SetMinMaxX(MinX, ResponseFunctions[0].MaxX, MinX, MaxX);
        } 
        else if (functionIndex == Segments.Count) // Removing last function
        {
            ResponseFunctions.Remove(responseFunction);
            RemoveSegment(Segments[removeIndex]);
            var lastFuncion = ResponseFunctions.Last();
            //ResponseFunctions.Last().SetMaxX(MaxX, MaxX);
            lastFuncion.SetMinMaxX(lastFuncion.MinX, MaxX, MinX, MaxX);
        }
        else
        {
            RemoveSegment(Segments[removeIndex]);
            ResponseFunctions.Remove(responseFunction);

            if (Segments.Count > 0)
            {
                var segmentToUpdate = Segments[removeIndex];
                segmentDisposables[segmentToUpdate].Dispose();

                var firstFunction = ResponseFunctions[removeIndex];
                var lastFunction = ResponseFunctions[removeIndex + 1];

                var sub = segmentToUpdate.OnValueChange
                    .Subscribe(value =>
                    {
                        var valueCast = Convert.ToSingle(value);
                        firstFunction.SetMinMaxX(firstFunction.MinX, valueCast, MinX, MaxX);
                        lastFunction.SetMinMaxX(valueCast,lastFunction.MinX, MinX, MaxX);
                        //firstFunction.SetMaxX(valueCast, MaxX);
                        //lastFunction.SetMinX(valueCast, MaxX - MinX);
                    });
                segmentDisposables[segmentToUpdate] = sub;
                var segmentValue = (lastFunction.MinX - firstFunction.MaxX) / 2;
                firstFunction.SetMinMaxX(firstFunction.MinX, segmentValue, MinX, MaxX);
                lastFunction.SetMinMaxX(segmentValue, lastFunction.MinX, MinX, MaxX);
                //firstFunction.SetMaxX(segmentValue, MaxX);
                //lastFunction.SetMinX(segmentValue, MaxX - MinX);


            }
        }
        onValuesChanged.OnNext(true);
    }

    internal void UpdateFunction(ResponseFunction oldFunction, ResponseFunction newFunction)
    {
        var oldFunctionIndex = ResponseFunctions.IndexOf(oldFunction);
        newFunction.SetMinMaxX(oldFunction.MinX, oldFunction.MaxX, MinX, MaxX);
        //newFunction.SetMinX(oldFunction.MinX, MaxX - MinX);
        //newFunction.SetMaxX(oldFunction.MaxX, MaxX);
        ResponseFunctions[oldFunctionIndex] = newFunction;
        UpdateSegmentSubscriptions();

        onValuesChanged.OnNext(true);
    }

    private void UpdateSegmentSubscriptions()
    {
        foreach(var segment in Segments)
        {
            segmentDisposables[segment].Dispose();
            var segmentIndex = Segments.IndexOf(segment);
            var previousFunction = ResponseFunctions[segmentIndex];
            var nextFunction = ResponseFunctions[segmentIndex + 1];

            var sub = segment.OnValueChange
                    .Subscribe(value =>
                    {
                        var valueCast = Convert.ToSingle(value);
                        previousFunction.SetMinMaxX(valueCast,previousFunction.MaxX,MinX,MaxX);
                        nextFunction.SetMinMaxX(nextFunction.MinX, valueCast, MinX, MaxX);
                        //previousFunction.SetMaxX(valueCast, MaxX);
                        //nextFunction.SetMinX(valueCast, MaxX - MinX);
                    });

            segmentDisposables[segment] = sub;
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
        }
        if (x > lastValidFunction.MinX)
        {
            result += lastValidFunction.GetResponseValue(x);
        }

        return Mathf.Clamp(result,0,1);
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
        get => minX; set
        {
            minX = value;
            onValuesChanged.OnNext(true);
        }
    }
    public float MaxX
    {
        get => maxX;
        set
        {
            maxX = value;
            onValuesChanged.OnNext(true);
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