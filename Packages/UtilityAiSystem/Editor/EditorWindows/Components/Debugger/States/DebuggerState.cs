using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

internal abstract class DebuggerState
{
    protected DebuggerComponent DebuggerComponent;
    protected TemplateContainer Root;

    protected Label InfoLabelLeft;

    protected Button BackLeapButton;
    protected Button BackStepButton;
    protected Button ToggleStateButton;
    protected Button ForwardStepButton;
    protected Button ForwardLeapButton;

    protected SliderInt TickSlider;

    protected Toggle RecordToggle;

    protected VisualElement Body;
    protected AgentComponent AgentComponent;

    public IAgent Agent { get; protected set; }
    protected Ai PlayAi;

    protected int CurrentTick => TickSlider.value;

    protected DebuggerState(TemplateContainer root, DebuggerComponent debuggerComponent, IAgent agent)
    {
        DebuggerComponent = debuggerComponent;
        this.Root = root;
        Agent = agent;
        Init();
    }

    protected virtual void Init()
    {
        InfoLabelLeft = Root.Q<Label>("InfoLeft-Label");

        BackLeapButton = Root.Q<Button>("BackLeapButton");
        BackStepButton = Root.Q<Button>("BackStepButton");
        ToggleStateButton = Root.Q<Button>("ToggleButton");
        ForwardStepButton = Root.Q<Button>("ForwardStepButton");
        ForwardLeapButton = Root.Q<Button>("ForwardLeapButton");

        TickSlider = Root.Q<SliderInt>("Tick-Slider");

        RecordToggle = Root.Q<Toggle>("Record-Toggle");

        Body = Root.Q<VisualElement>("Body");

        if (AgentComponent == null)
        {
            Body.Clear();
            AgentComponent = new AgentComponent(Agent);
            Body.Add(AgentComponent);
        }
    }

    internal abstract void UpdateUi();
    internal virtual void UpdateAgent(IAgent agent)
    {
        Agent = agent;
        if (agent == null)
        {
            AgentComponent = null;
            Body.Clear();
        }
        else
        {

            if (AgentComponent == null)
            {
                Body.Clear();
                AgentComponent = new AgentComponent(agent);
                Body.Add(AgentComponent);
            }
            else
            {
                AgentComponent.UpdateAgent(agent);
            }
        }

        //DebuggerComponent.UpdateAgent(agent);
    }

    internal virtual void OnEnter() { }
    internal virtual void OnExit() { }

    internal virtual void BackLeapButtonPressed()
    {

    } 
    
    internal virtual void BackStepButtonPressed()
    {
    }

    internal virtual void ToggleStateButtonPressed()
    {
        EditorApplication.isPlaying = true;
        EditorApplication.isPaused = !EditorApplication.isPaused;

    }

    internal virtual void ForwardStepButtonPressed()
    {
    }

    internal virtual void ForwardLeapButtonPressed()
    {
    }

    internal virtual void TickSliderChanged(int newValue)
    {
        TickSlider.label = newValue.ToString();
    }

}