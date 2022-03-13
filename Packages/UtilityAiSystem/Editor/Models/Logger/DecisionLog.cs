using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DecisionLog: AiObjectLog
{
    internal float Score = 0f;
    internal List<ConsiderationLog> Considerations = new List<ConsiderationLog>();
    internal List<AgentActionLog> AgentActions = new List<AgentActionLog>();
    internal List<ParameterLog> Parameters = new List<ParameterLog>();

    internal static DecisionLog GetDebug(Decision decision)
    {
        var result = new DecisionLog();
        result.Name = decision.Name;
        result.Description = decision.Description;
        result.Type = decision.GetType().ToString();
        result.Score = decision.ScoreModels.First().Value;

        result.Considerations = new List<ConsiderationLog>();
        foreach(var consideration in decision.Considerations.Values)
        {
            result.Considerations.Add(ConsiderationLog.GetDebug(consideration));
        }

        result.Parameters = new List<ParameterLog>();
        foreach(var parameter in decision.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(parameter));
        }

        result.AgentActions = new List<AgentActionLog>();
        foreach(var agentAction in decision.AgentActions.Values)
        {
            result.AgentActions.Add(AgentActionLog.GetDebug(agentAction));
        }

        return result;
    }
}
