using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal abstract class AiObjectLog: ILogModel
{
    public string Name = "";
    public string Description = "";
    public string Type = "";
    public int LastSelectedTick;
    public int LastEvaluatedTick;
    public int CurrentTick;

    internal static AiObjectLog SetBasics(AiObjectLog log, AiObjectModel model, int tick)
    {
        log.Name = model.Name;
        log.Description = model.Description;
        log.Type = model.GetType().ToString();
        log.LastSelectedTick = model.MetaData.LastTickSelected;
        log.LastEvaluatedTick = model.MetaData.LastTickEvaluated;
        log.CurrentTick = tick;
        return log;
    }

    internal static AiObjectLog SetBasics(AiObjectLog log, IAgent agent, int tick)
    {
        log.Name = agent.Model.Name;
        log.Description = "";
        log.Type = agent.GetType().ToString();
        log.LastSelectedTick = agent.Model.TickMetaData.TickCount;
        log.CurrentTick = tick;
        return log;
    }
}
