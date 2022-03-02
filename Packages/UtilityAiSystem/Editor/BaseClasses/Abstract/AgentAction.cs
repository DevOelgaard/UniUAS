using System;
using System.Collections.Generic;
using System.ComponentModel;

public abstract class AgentAction: AiObjectModel
{
    public List<Parameter> Parameters;
    private string namePostfix;

    protected AgentAction()
    {
        Parameters = new List<Parameter>(GetParameters());
        namePostfix = " (" + TypeDescriptor.GetClassName(this) + ")";
    }

    protected abstract List<Parameter> GetParameters();

    public virtual void OnStart(AiContext context) { }
    public virtual void Ongoing(AiContext context) { }
    public virtual void OnEnd(AiContext context) { }

    public override string GetNameFormat(string name)
    {
        if (!name.Contains(namePostfix))
        {
            return name + namePostfix;
        }
        return name;
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<AgentAction>(state);
        return clone;
    }

    internal AgentActionState GetState()
    {
        return new AgentActionState(Parameters, Name, Description, this);
    }

    protected override void RestoreInternal(RestoreState s)
    {
        var state = (AgentActionState)s;
        Name = state.Name;
        Description = state.Description;
        foreach (var p in state.Parameters)
        {
            var parameter = Parameter.Restore<Parameter>(p);
            Parameters.Add(parameter);
        }
    }
}

[Serializable]
public class AgentActionState: RestoreState
{
    public List<ParameterState> Parameters;
    public string Name;
    public string Description;

    public AgentActionState(): base()
    {
    }

    public AgentActionState(List<Parameter> parameters, string name, string description, AgentAction action): base(action)
    {
        Name = name;
        Description = description;

        Parameters = new List<ParameterState>();
        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter.GetState());
        }
    }
}
