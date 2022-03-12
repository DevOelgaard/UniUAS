using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

internal class DebuggerGamePausedLogs : GamePausedNestedState
{
    private HelpBox helpBox;
    private AiLogComponent aiLogComponent;
    public DebuggerGamePausedLogs(TemplateContainer root, DebuggerComponent debuggerComponent) 
        : base(root, debuggerComponent)
    {
        helpBox = new HelpBox("AI logs for the selected agent at this tick", HelpBoxMessageType.Info);
        Body.Add(helpBox);
        helpBox.style.opacity = 0;
        aiLogComponent = new AiLogComponent();
    }

    internal override void OnEnter(IAgent agent)
    {
        base.OnEnter(agent);
        AgentComponent.style.opacity = 0;
        InfoLabelLeft.text = "Ai logs";
        RecordToggle.text = "Logs";
        InspectAi(CurrentTick);
    }

    internal override void OnExit()
    {
        base.OnExit();
        AgentComponent.style.opacity = 1;
        RecordToggle.text = "";
    }

    internal override void TickChanged(int value)
    {
        base.TickChanged(value);
        InspectAi(value);
    }

    private void InspectAi(int tick)
    {
        var aiLog = AiLoggerService.Instance.GetAiDebugLog(Agent, tick);

        if (aiLog == null)
        {
            aiLogComponent.Hide();
            helpBox.style.opacity = 1;
        }
        else
        {
            helpBox.style.opacity = 0;
            aiLogComponent.Display(aiLog);
        }
    }
}