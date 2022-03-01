using System.Collections.Generic;
using UniRxExtension;

public class AgentManager
{
    private static AgentManager instance;
    private AgentManagerModel model = new AgentManagerModel();
    public List<string> AgentTypes => model.AgentTypes;


    public AgentManager()
    {
    }

    public static AgentManager Instance { 
        get 
        { 
            if (instance == null)
            {
                instance = new AgentManager();
            } 
            return instance; 
        } 
    }

    public void Register(IAgent agent)
    {
        var type = agent.GetType().ToString();
        if (!model.AgentsByType.ContainsKey(type))
        {
            model.AgentsByType.Add(type, new ReactiveList<IAgent>());
            AgentTypes.Add(type);
        }

        agent.Model.Name = agent.Model.Name + " " + model.AgentsByType[type].Count;

        model.AgentsByType[type].Add(agent);
    }

    public void Unregister(IAgent agent)
    {
        var type = agent.GetType().ToString();
        if (model.AgentsByType.ContainsKey(type))
        {
            model.AgentsByType.Remove(type);
        }
    }

    public ReactiveList<IAgent> GetAgentsByType(string type)
    {
        if (!model.AgentsByType.ContainsKey(type))
        {
            return new List<IAgent>();
        } else
        {
            return model.AgentsByType[type];
        }
    }
}