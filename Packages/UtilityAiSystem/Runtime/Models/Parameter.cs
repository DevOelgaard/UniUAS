using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class Parameter: RestoreAble
{
    public string Name;
    private object v;
    public object Value { 
        get => v;
        set
        {
            v = value;
            valueChanged.OnNext(v);
        }
    }

    public ParameterEnum ParameterEnum;

    public IObservable<object> OnValueChange => valueChanged;
    private Subject<object> valueChanged = new Subject<object>();

    public Parameter()
    {
        Name = "";
    }

    public Parameter(string name, object value, ParameterEnum pEnum)
    {
        Name = name;
        Value = value;
        ParameterEnum = pEnum;
    }

    internal override RestoreState GetState()
    {
        return new ParameterState(Name, v, this);
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (ParameterState)s;
        Name = state.Name;
        v = state.Value;
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
}

[Serializable]
public class ParameterState: RestoreState
{
    public string Name;
    public object Value;

    public ParameterState(): base()
    {
    }

    public ParameterState(string name, object v, Parameter p): base(p)
    {
        Name = name;
        this.Value = v;
    }
}
