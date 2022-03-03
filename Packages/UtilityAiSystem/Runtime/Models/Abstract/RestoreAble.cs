using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class RestoreAble
{
    protected abstract void RestoreInternal(RestoreState state);
    public static T Restore<T>(RestoreState state) where T:RestoreAble
    {
        Debug.Log("State: " + state.FileName);
        var type = Type.GetType(state.FileName);
        if (type == null)
        {
            var e = AssetDatabaseService.GetInstanceOfType<T>(state.FileName);
            e.RestoreInternal(state);
            return e;
        } else
        {
            var element = (T)Activator.CreateInstance(type, true);
            element.RestoreInternal(state);
            return element;
        }
    }
}

public class RestoreState
{
    public string FileName;

    public RestoreState()
    {
    }

    public RestoreState(RestoreAble o)
    {
        FileName = TypeDescriptor.GetClassName(o);
    }
}
