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
    private AgentComponent agentComponent;
    protected AgentComponent AgentComponent
    {
        get
        {
            if (agentComponent == null)
            {
                agentComponent = Root.Query<AgentComponent>().First();
            }
            return agentComponent;
        }
    }

    public IAgent Agent { get; protected set; }
    protected Ai PlayAi;

    protected int CurrentTick => TickSlider.value;

    protected DebuggerState(TemplateContainer root, DebuggerComponent debuggerComponent)
    {
        DebuggerComponent = debuggerComponent;
        this.Root = root;
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

        RecordToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            RecordToggleChanged(evt.newValue);
        });
    }

    internal virtual void UpdateAgent(IAgent agent)
    {
        Agent = agent;
        if (agent == null)
        {
            if (AgentComponent != null)
            {
                AgentComponent.style.display = DisplayStyle.None;
                //AgentComponent.style.opacity = 0;
            }
        }
        else
        {
            AgentComponent.style.display = DisplayStyle.Flex;

            //AgentComponent.style.opacity = 1;
            AgentComponent.UpdateAgent(agent);
        }
    }

    internal virtual void OnEnter(IAgent agent) { 
        UpdateAgent(agent);
    }
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
        //TickSlider.highValue = AiTicker.Instance.TickCount;
        TickSlider.label = newValue.ToString();
    }

    internal virtual void RecordToggleChanged(bool value)
    {

    }
}