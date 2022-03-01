using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AgentMono : MonoBehaviour, IAgent
{
    private AgentModel model = new AgentModel();
    public AgentModel Model => model;

    void Start()
    {
        AgentManager.Instance.Register(this);
    }


    void OnDestroy()
    {
        AgentManager.Instance?.Unregister(this);
    }
}
