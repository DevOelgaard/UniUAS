using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;
using UniRx;
using System;

public class AgentManager
{
    private static AgentManager instance;
    private AgentManagerModel model = new AgentManagerModel();
    public List<string> AgentIdentifiers => model.AgentTypes;

    public IObservable<bool> AgentTypesUpdated => agentIdentifiersUpdated;
    private Subject<bool> agentIdentifiersUpdated = new Subject<bool>();

    public IObservable<IAgent> AgentsUpdated => agentsUpdated;
    private Subject<IAgent> agentsUpdated = new Subject<IAgent>();
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
        var identifier = agent.Identifier;
        if (!model.AgentsByIdentifier.ContainsKey(identifier))
        {
            model.AgentsByIdentifier.Add(identifier, new ReactiveList<IAgent>());
            AgentIdentifiers.Add(identifier);
            agentIdentifiersUpdated.OnNext(true);

        }

        agent.Model.Name += " " + model.AgentsByIdentifier[identifier].Count;

        model.AgentsByIdentifier[identifier].Add(agent);
        agentsUpdated.OnNext(agent);
        Debug.Log("Registering : " + agent.Model);
    }

    public void Unregister(IAgent agent)
    {
        var identifier = agent.Identifier;
        if (model.AgentsByIdentifier.ContainsKey(identifier))
        {
            model.AgentsByIdentifier.Remove(identifier);
        }
        agentsUpdated.OnNext(agent);
    }

    public ReactiveList<IAgent> GetAgentsByIdentifier(string identifier)
    {
        if (!model.AgentsByIdentifier.ContainsKey(identifier))
        {
            return new ReactiveList<IAgent>();
        } else
        {
            return model.AgentsByIdentifier[identifier];
        }
    }
}