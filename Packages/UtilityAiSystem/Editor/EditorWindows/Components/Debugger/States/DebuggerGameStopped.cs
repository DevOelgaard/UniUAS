using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

internal class DebuggerGameStopped : DebuggerState
{
    public DebuggerGameStopped(TemplateContainer root, DebuggerComponent debuggerComponent, IAgent agent)
        : base(root, debuggerComponent, agent)
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
        InfoLabelLeft.text = "Game Stopped";
        ToggleStateButton.text = "Play";
    }

    internal override void OnExit()
    {
        base.OnExit();
        BackLeapButton.SetEnabled(true);
        BackStepButton.SetEnabled(true);
        ForwardStepButton.SetEnabled(true);
        ForwardLeapButton.SetEnabled(true);
        TickSlider.SetEnabled(true);
    }

    internal override void UpdateUi()
    {
    }

    internal override void ToggleStateButtonPressed()
    {
        EditorApplication.isPlaying = true;
    }
}
