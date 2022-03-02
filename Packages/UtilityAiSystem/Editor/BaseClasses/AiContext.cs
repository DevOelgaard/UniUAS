using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class AiContext
{
    public IAgent Agent;
    public AgentAction CurrentAction;
    private Dictionary<string, object> context = new Dictionary<string, object>();
    public IUtilityScorer UtilityScorer = new USAverageScorer();
    //public IUtilityContainerSelector BucketSelector = new UCSHighestScore();
    //public IUtilityContainerSelector DecisionSelector = new UCSHighestScore();
         
    public AiContext()
    {
    }

    public object GetContext(string key)
    {
        if (context.ContainsKey(key))
        {
            return context[key];
        } else
        {
            return null;
        }
    }

    public void SetContext(string key, object value)
    {
        if (!context.ContainsKey(key))
        {
            context.Add(key, value);
        } else
        {
            context[key] = value;
        }
    }

    public void RemoveContext(string key)
    {
        if (context.ContainsKey(key))
        {
            context.Remove(key);
        }
    }
}
