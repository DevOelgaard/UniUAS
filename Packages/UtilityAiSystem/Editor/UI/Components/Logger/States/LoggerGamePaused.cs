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
        BackLeapButton.SetEnabled(true);
        BackStepButton.SetEnabled(true);
        ForwardStepButton.SetEnabled(true);
        ForwardLeapButton.SetEnabled(true);
        ToggleStateButton.text = "Resume";
        InfoLabelLeft.text = "Game Paused";
        RecordToggle.text = "Inspect";
    }


    internal override void OnExit()
    {
        base.OnExit();
    }

    internal override void UpdateUi(IAgent agent)
    {
        Agent = agent;
        if (agent != null)
        {
            InspectAi(CurrentTick);
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

    protected override void SetCurrentTick(int tick)
    {
        if (tick > CurrentTick)
        {
            AiTicker.Instance.TickUntilCount(tick,true);
        }
        base.SetCurrentTick(tick);
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