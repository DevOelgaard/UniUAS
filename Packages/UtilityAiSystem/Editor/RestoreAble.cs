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
        var type =   Type.GetType(state.FileName);
        // Type is from another assembly
        if (type == null)
        {
            var types = AssetDatabaseService.GetInstancesOfType<T>();
            var e = types.FirstOrDefault(t => t.GetType().FullName == state.FileName);
            e.RestoreInternal(state);
            return e;
        }
        Debug.Log("N: " + state.FileName + " T:" + type);
        var element = (T)Activator.CreateInstance(type, true);
        element.RestoreInternal(state);
        return element;
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
