using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEditor;

internal class DebuggerGameRunning : DebuggerState
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public DebuggerGameRunning(TemplateContainer root, DebuggerComponent debuggerComponent): base(root, debuggerComponent)
    {

    }

    internal override void OnEnter()
    {
        base.OnEnter();
        BackLeapButton.SetEnabled(false);
        BackStepButton.SetEnabled(false);
        ForwardStepButton.SetEnabled(false);
        ForwardLeapButton.SetEnabled(false);
        TickSlider.SetEnabled(false);

        TickSlider.value = AiTicker.Instance.TickCount;
        ToggleStateButton.text = "Pause";
        InfoLabelLeft.text = "Game Running";
        AiTicker.Instance
            .OnTickComplete
            .Subscribe(tick =>
            {
                TickSlider.value = tick;
                if (RecordToggle.value)
                {
                    AiDebuggerService.Instance.LogTick(Agent, tick);
                }
            })
            .AddTo(disposables);
    }

    internal override void OnExit()
    {
        BackLeapButton.SetEnabled(true);
        BackStepButton.SetEnabled(true);
        ForwardStepButton.SetEnabled(true);
        ForwardLeapButton.SetEnabled(true);
        TickSlider.SetEnabled(true);
    }

    internal override void UpdateAgent(IAgent agent)
    {
        base.UpdateAgent(agent);
    }
    internal override void UpdateUi()
    {
    }

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }

    ~DebuggerGameRunning()
    {
        ClearSubscriptions();
    }
}