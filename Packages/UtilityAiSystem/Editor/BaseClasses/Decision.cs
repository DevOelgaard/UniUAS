﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;

public class Decision: UtilityContainer
{
    public ReactiveList<AgentAction> AgentActions = new ReactiveList<AgentAction>();

    public Decision()
    {
    }

    public DecisionState GetState()
    {
        return new DecisionState(Name, Description, AgentActions.Values, Considerations.Values, this);
    }

    internal override MainWindowModel Clone()
    {
        var state = GetState();
        var clone = Restore<Decision>(state);
        return clone;
    }

    protected override void RestoreInternal(RestoreState s)
    {
        var state = (DecisionState)s;
        Name = state.Name;
        Description = state.Description;

        AgentActions = new ReactiveList<AgentAction>();
        foreach (var a in state.AgentActions)
        {
            var action = AgentAction.Restore<AgentAction>(a);
            AgentActions.Add(action);
        }

        Considerations = new ReactiveList<Consideration>();
        foreach (var c in state.Considerations)
        {
            var consideration = Consideration.Restore<Consideration>(c);
            Considerations.Add(consideration);
        }
    }
}

[Serializable]
public class DecisionState: RestoreState
{
    public string Name;
    public string Description;
    public List<AgentActionState> AgentActions;
    public List<ConsiderationState> Considerations;

    public DecisionState() : base()
    {
    }

    public DecisionState(string name, string description, List<AgentAction> agentActions, List<Consideration> considerations, RestoreAble o) : base(o)
    {
        Name = name;
        Description = description;

        AgentActions = new List<AgentActionState>();
        foreach (AgentAction action in agentActions)
        {
            var a = action.GetState();
            AgentActions.Add(a);
        }

        Considerations = new List<ConsiderationState>();
        foreach (Consideration consideration in considerations)
        {
            var c = consideration.GetState();
            Considerations.Add(c);
        }
    }
}
