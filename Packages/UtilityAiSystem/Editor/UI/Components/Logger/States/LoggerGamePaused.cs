using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

internal class LoggerGamePaused : LoggerState
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public LoggerGamePaused(TemplateContainer root, LoggerComponent debuggerComponent) 
        : base(root, debuggerComponent)
    {

    }

    internal override void OnEnter(IAgent agent)
    {
        base.OnEnter(agent);
        TickAgentButton.SetEnabled(false);
        ToggleStateButton.text = "Resume";
        InfoLabelLeft.text = "Game Paused";
        RecordToggle.text = "Inspect";
    }


    internal override void OnExit()
    {
        base.OnExit();
        if (Agent != null && PlayAi != null)
        {
            Agent.Ai = PlayAi;
        }
    }

    internal override void UpdateUi(IAgent agent)
    {
        Agent = agent;
        if (agent != null)
        {
            PlayAi = agent.Ai;
        }
    }

    internal override void BackLeapButtonPressed()
    {
        SetCurrentTick(CurrentTick - ConstsEditor.Debugger_LeapSize);

    }

    internal override void BackStepButtonPressed()
    {
        SetCurrentTick(CurrentTick - ConstsEditor.Debugger_StepSize);
    }

    internal override void ForwardStepButtonPressed()
    {
        SetCurrentTick(CurrentTick + ConstsEditor.Debugger_StepSize);
    }

    internal override void ForwardLeapButtonPressed()
    {
        SetCurrentTick(CurrentTick + ConstsEditor.Debugger_LeapSize);
    }

    internal override void TickSliderChanged(int newValue)
    {
        base.TickSliderChanged(newValue);
        SetCurrentTick(newValue);
    }

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }


    ~LoggerGamePaused()
    {
        ClearSubscriptions();
    }
}