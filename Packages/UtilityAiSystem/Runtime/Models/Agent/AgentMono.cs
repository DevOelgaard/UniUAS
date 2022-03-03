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
    private UAIModel ai;
    public UAIModel Ai
    {
        get => ai;
        set
        {
            ai = value;
            ai.Context.Agent = this;
        }
    }

    void Start()
    {
        model.Name = SetAgentName();
        AgentManager.Instance.Register(this);
        var ai = UASTemplateService.Instance.GetAiByName(DefaultAiName);
        SetAi(ai);
        decisionScoreEvaluator = new DecisionScoreEvaluator();
    }

    public void SetAi(UAIModel model)
    {
        Ai = model;
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

    public void Tick(TickMetaData metaData)
    {
        Ai.Context.SetContext(AiContextKey.TickMetaData, metaData);
        var actions = decisionScoreEvaluator.NextActions(Ai.Buckets.Values, Ai.Context);
        foreach(var action in actions)
        {
            action.OnStart(Ai.Context);
        }
    }
}
