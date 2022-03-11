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
    private int stepSize = 1;
    private int leapSize = 10;
    private bool isDisplayingDebug = false;
    public DebuggerGamePaused(TemplateContainer root, DebuggerComponent debuggerComponent, IAgent agent) 
        : base(root, debuggerComponent, agent)
    {
    }

    internal override void OnEnter()
    {
        base.OnEnter();
        ToggleStateButton.text = "Resume";
        InfoLabelLeft.text = "Game Paused";
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
    }

    internal override void UpdateUi()
    {
        throw new NotImplementedException();
    }

    internal override void BackLeapButtonPressed()
    {
        TickSlider.value -= leapSize;

    }

    internal override void BackStepButtonPressed()
    {
        TickSlider.value -= stepSize;

    }

    internal override void ForwardStepButtonPressed()
    {
        TickSlider.value += stepSize;

    }

    internal override void ForwardLeapButtonPressed()
    {
        TickSlider.value += leapSize;
    }

    internal override void TickSliderChanged(int newValue)
    {
        base.TickSliderChanged(newValue);
        InspectAi(newValue);
    }

    private void InspectAi(int tick)
    {
        var ai = AiDebuggerService.Instance.GetAiDebugLog(Agent,tick);
        if (ai == null)
        {
            Debug.Log("No ai at: " + tick);
        } else
        {
            Body.Clear();
            Body.Add(new AiDebugComponent(ai));
            //AgentComponent.UpdateAgent(Agent);
        }
    }

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }

    ~DebuggerGamePaused()
    {
        ClearSubscriptions();
    }
}