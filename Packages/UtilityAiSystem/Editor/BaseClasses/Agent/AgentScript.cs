using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AgentScript : IAgent
{
    private AgentModel model = new AgentModel();
    public AgentModel Model => model;

    public string TypeIdentifier => GetType().FullName;

    [SerializeField]
    protected string DefaultAiName => GetDefaultAiName();

    public AgentScript()
    {
        model.Name = SetAgentName();
        model.AI = UASTemplateService.Instance.GetAiByName(DefaultAiName);
        AgentManager.Instance.Register(this);
    }

    ~AgentScript()
    {
        AgentManager.Instance?.Unregister(this);
    }

    /// <summary>
    /// Returns the desired AiAgent name, which is displayd in the UAS Tools.
    /// By default set as class name
    /// </summary>
    /// <returns></returns>
    protected virtual string SetAgentName()
    {
        return TypeIdentifier;
    }

    protected virtual string GetDefaultAiName()
    {
        return "";
    }
}