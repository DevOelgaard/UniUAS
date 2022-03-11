using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DecisionDebug: AiObjectDebug
{
    internal float Score = 0f;
    internal List<ConsiderationDebug> Considerations = new List<ConsiderationDebug>();
    internal List<AgentActionDebug> AgentActions = new List<AgentActionDebug>();
    internal ParametersDebug Parameters;

    internal static DecisionDebug GetDebug(Decision decision)
    {
        var result = new DecisionDebug();
        result.Name = decision.Name;
        result.Description = decision.Description;
        result.Type = decision.GetType().ToString();
        result.Score = decision.ScoreModels.First().Value;

        result.Considerations = new List<ConsiderationDebug>();
        foreach(var consideration in decision.Considerations.Values)
        {
            result.Considerations.Add(ConsiderationDebug.GetDebug(consideration));
        }

        result.Parameters = new ParametersDebug(decision.Parameters);
        return result;
    }
}
