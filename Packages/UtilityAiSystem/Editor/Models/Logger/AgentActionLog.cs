using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionLog: AiObjectLog
{
    internal List<ParameterLog> Parameters;

    internal static AgentActionLog GetDebug(AgentAction aa)
    {
        var result = new AgentActionLog();
        result.Name = aa.Name;
        result.Description = aa.Description;
        result.Type = aa.GetType().ToString();
        result.Parameters = new List<ParameterLog>();
        foreach (var p in aa.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p));
        }
        return result;
    }
}
