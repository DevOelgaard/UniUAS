using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AgentScript : IAgent
{
    private AgentModel model = new AgentModel();
    public AgentModel Model => model;

    public AgentScript()
    {
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
        return GetType().FullName;
    }
}