using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

internal class DebuggerGamePaused : DebuggerState
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private bool isDisplayingDebug = false;
    private GamePausedNestedState state;
    public DebuggerGamePaused(TemplateContainer root, DebuggerComponent debuggerComponent) 
        : base(root, debuggerComponent)
    {

    }

    internal override void OnEnter(IAgent agent)
    {
        base.OnEnter(agent);
        ToggleStateButton.text = "Resume";
        InfoLabelLeft.text = "Game Paused";
        RecordToggle.text = "Inspect";
        UpdateState();
    }


    internal override void OnExit()
    {
        base.OnExit();
        if (Agent != null && PlayAi != null)
        {
            Agent.Ai = PlayAi;
        }
        if (isDisplayingDebug)
        {
            Body.Clear();
            Body.Add(AgentComponent);
        }
    }

    internal override void UpdateAgent(IAgent agent)
    {
        base.UpdateAgent(agent);
        if (agent != null)
        {
            PlayAi = agent.Ai;
        }
        state?.UpdateAgent(agent);
    }

    internal override void BackLeapButtonPressed()
    {
        TickSlider.value -= ConstsEditor.Debugger_LeapSize;

    }

    internal override void BackStepButtonPressed()
    {
        TickSlider.value -= ConstsEditor.Debugger_StepSize;

    }

    internal override void ForwardStepButtonPressed()
    {
        TickSlider.value += ConstsEditor.Debugger_StepSize;

    }

    internal override void ForwardLeapButtonPressed()
    {
        TickSlider.value += ConstsEditor.Debugger_LeapSize;
    }

    internal override void TickSliderChanged(int newValue)
    {
        base.TickSliderChanged(newValue);
        state.TickChanged(newValue);
    }

    private void SetState(GamePausedNestedState state)
    {
        this.state?.OnExit();
        this.state = state;
        this.state.OnEnter(Agent);
    }

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }

    internal override void RecordToggleChanged(bool value)
    {
        base.RecordToggleChanged(value);
        UpdateState();
    }

    private void UpdateState()
    {
        if (RecordToggle.value)
        {
            SetState(new DebuggerGamePausedLogs(Root, DebuggerComponent));
        }
        else
        {
            SetState(new DebuggerGamePausedInspect(Root, DebuggerComponent));
        }
    }

    ~DebuggerGamePaused()
    {
        ClearSubscriptions();
    }
}