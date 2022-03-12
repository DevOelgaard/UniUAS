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
    public DebuggerGameRunning(TemplateContainer root, DebuggerComponent debuggerComponent)
        : base(root, debuggerComponent)
    {

    }

    internal override void OnEnter(IAgent agent)
    {
        base.OnEnter(agent);
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
            .Subscribe(latestTick =>
            {
                TickSlider.highValue = latestTick;
                TickSlider.value = latestTick;
                if (RecordToggle.value)
                {
                    AiLoggerService.Instance.LogTick(Agent, latestTick);
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

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }

    ~DebuggerGameRunning()
    {
        ClearSubscriptions();
    }
}