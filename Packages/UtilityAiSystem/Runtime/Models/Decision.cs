﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;

public class Decision: UtilityContainer
{
    private IDisposable agentActionSub;
    private ReactiveList<AgentAction> agentActions = new ReactiveList<AgentAction>();
    public ReactiveList<AgentAction> AgentActions
    {
        get=> agentActions;
        set
        {
            agentActions = value;
            if (agentActions != null)
            {
                agentActionSub?.Dispose();
                UpdateInfo();
                agentActionSub = agentActions.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }

    public Decision()
    {
        agentActionSub?.Dispose();
        UpdateInfo();
        agentActionSub = agentActions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
    }

    protected override void UpdateInfo()
    {
        base.UpdateInfo();
        if (AgentActions.Count <= 0)
        {
            Info = new InfoModel("No AgentActions, Object won't be selected", InfoTypes.Warning);
        }
        else if (Considerations.Count <= 0)
        {
            Info = new InfoModel("No Considerations, Object won't be selected", InfoTypes.Warning);
        }
        else
        {
            Info = new InfoModel();
        }
    }

    internal override RestoreState GetState()
    {
        return new DecisionState(Name, Description, AgentActions.Values, Considerations.Values, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }

    internal override AiObjectModel Clone()
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
    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        agentActionSub?.Dispose();
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
            var a = action.GetState() as AgentActionState;
            AgentActions.Add(a);
        }

        Considerations = new List<ConsiderationState>();
        foreach (Consideration consideration in considerations)
        {
            var c = consideration.GetState() as ConsiderationState;
            Considerations.Add(c);
        }
    }

}