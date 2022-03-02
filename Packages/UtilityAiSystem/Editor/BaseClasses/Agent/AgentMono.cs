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

    public string TypeIdentifier => GetType().FullName;

    public string DefaultAiName = "";

    private DecisionScoreEvaluator decisionScoreEvaluator;

    void Start()
    {
        model.Name = SetAgentName();
        model.AI = UASTemplateService.Instance.GetAiByName(DefaultAiName);
        model.AI.Context.Agent = this;
        AgentManager.Instance.Register(this);
        decisionScoreEvaluator = new DecisionScoreEvaluator();
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

    public void Tick()
    {
        var actions = decisionScoreEvaluator.NextActions(Model.AI.Buckets.Values, Model.AI.Context);
        foreach(var action in actions)
        {
            action.OnStart(Model.AI.Context);
        }
    }
}
