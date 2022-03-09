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
            newFunction.MinX = MinX;
            newFunction.MaxX = MaxX;
        } else
        {
            var segmentValue = (float)lastFunction.MaxX / 2;
            var newSegment = new Parameter(" ", segmentValue);
            var segmentSub = newSegment.OnValueChange
                .Subscribe(value =>
                {
                    lastFunction.MaxX = Convert.ToSingle(value);
                    newFunction.MinX = Convert.ToSingle(value);
                });

            segmentDisposables.Add(newSegment,segmentSub);

            lastFunction.MaxX = segmentValue;
            newFunction.MinX = segmentValue;

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
            ResponseFunctions[0].MinX = MinX;
        } 
        else if (functionIndex == Segments.Count) // Removing last function
        {
            ResponseFunctions.Remove(responseFunction);
            RemoveSegment(Segments[removeIndex]);
            ResponseFunctions.Last().MaxX = MaxX;
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
                        firstFunction.MaxX = Convert.ToSingle(value);
                        lastFunction.MinX = Convert.ToSingle(value);
                    });
                segmentDisposables[segmentToUpdate] = sub;
                var segmentValue = (lastFunction.MinX - firstFunction.MaxX) / 2;
                firstFunction.MaxX = segmentValue;
                lastFunction.MinX = segmentValue;


            }
        }
        onValuesChanged.OnNext(true);
    }

    internal void UpdateFunction(ResponseFunction oldFunction, ResponseFunction newFunction)
    {
        var oldFunctionIndex = ResponseFunctions.IndexOf(oldFunction);
        newFunction.MinX = oldFunction.MinX;
        newFunction.MaxX = oldFunction.MaxX;
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
                        previousFunction.MaxX = Convert.ToSingle(value);
                        nextFunction.MinX = Convert.ToSingle(value);
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
        var normalizedX = Normalize(x);
        foreach(var funciton in ResponseFunctions)
        {
            if (x <= funciton.MaxX)
            {
                var response = funciton.CalculateResponse(normalizedX);
                //var result = response * MaxY + MinY;
                return Mathf.Clamp(response, MinY, MaxY);
            }
        }
        throw new Exception("This should not be reached");
    }

    private float Normalize(float value)
    {
        var x = (value - MinX) / (MaxX - MinX);
        return Mathf.Clamp(x, 0, 1);
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