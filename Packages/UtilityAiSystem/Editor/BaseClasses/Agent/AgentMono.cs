using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AgentMono : MonoBehaviour, IAgent
{
    private AgentModel model = new AgentModel();
    public AgentModel Model => model;

    public string Identifier => GetType().FullName;

    void Start()
    {
        model.Name = SetAgentName();
        AgentManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        AgentManager.Instance?.Unregister(this);
    }

    /// <summary>
    /// Returns the desired AiAgent name, which is displayd in the UAS Tools
    /// By default set as the name of the attached MonoBehaviour
    /// </summary>
    /// <returns></returns>
    protected virtual string SetAgentName()
    {
        return gameObject.name;
    }
}
