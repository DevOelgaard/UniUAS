﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class AiContext
{
    public IAgent Agent;
    public AgentAction CurrentAction;
    private Dictionary<string, object> contextStringKey = new Dictionary<string, object>();
    private Dictionary<AiContextKey, object> contextEnumKey = new Dictionary<AiContextKey, object>();
    public IUtilityScorer UtilityScorer = new USAverageScorer();
    //public IUtilityContainerSelector BucketSelector = new UCSHighestScore();
    //public IUtilityContainerSelector DecisionSelector = new UCSHighestScore();
         
    public AiContext()
    {
    }

    /// <summary>
    /// This is slower but more versatile. Consider using GetContext(AiContextKey) instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetContext(string key)
    {
        if (contextStringKey.ContainsKey(key))
        {
            return contextStringKey[key];
        } else
        {
            return null;
        }
    }

    /// <summary>
    /// This is slower but more versatile. Consider using SetContext(AiContextKey) instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void SetContext(string key, object value)
    {
        if (!contextStringKey.ContainsKey(key))
        {
            contextStringKey.Add(key, value);
        } else
        {
            contextStringKey[key] = value;
        }
    }

    /// <summary>
    /// This is slower but more versatile. Consider using RemoveContext(AiContextKey) instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void RemoveContext(string key)
    {
        if (contextStringKey.ContainsKey(key))
        {
            contextStringKey.Remove(key);
        }
    }

    /// <summary>
    /// Use this if you have defined the needed Enum in AiContextKey otherwise use the string version
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetContext(AiContextKey key)
    {
        if (contextEnumKey.ContainsKey(key))
        {
            return contextEnumKey[key];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Use this if you have defined the needed Enum in AiContextKey otherwise use the string version
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void SetContext(AiContextKey key, object value)
    {
        if (!contextEnumKey.ContainsKey(key))
        {
            contextEnumKey.Add(key, value);
        }
        else
        {
            contextEnumKey[key] = value;
        }
    }

    /// <summary>
    /// Use this if you have defined the needed Enum in AiContextKey otherwise use the string version
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void RemoveContext(AiContextKey key)
    {
        if (contextEnumKey.ContainsKey(key))
        {
            contextEnumKey.Remove(key);
        }
    }

}
