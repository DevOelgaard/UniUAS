using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;
using UniRx;
using System;

public class AgentManager
{
    private static AgentManager instance;
    private AgentManagerModel model = new AgentManagerModel();
    public List<string> AgentTypes => model.AgentTypes;

    public IObservable<bool> AgentTypesUpdated => agentTypesUpdated;
    private Subject<bool> agentTypesUpdated = new Subject<bool>();
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

        agent.Model.Name = " " + model.AgentsByType[type].Count;

        model.AgentsByType[type].Add(agent);
        Debug.Log("Registering : " + agent.Model);
        agentTypesUpdated.OnNext(true);
    }

    public void Unregister(IAgent agent)
    {
        var type = agent.GetType().ToString();
        if (model.AgentsByType.ContainsKey(type))
        {
            model.AgentsByType.Remove(type);
        }
        agentTypesUpdated.OnNext(true);
    }

    public ReactiveList<IAgent> GetAgentsByType(string type)
    {
        if (!model.AgentsByType.ContainsKey(type))
        {
            return new ReactiveList<IAgent>();
        } else
        {
            return model.AgentsByType[type];
        }
    }
}