using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

internal abstract class LoggerState
{
    protected LoggerComponent LoggerComponent;
    protected TemplateContainer Root;

    protected Label InfoLabelLeft;

    protected Button BackLeapButton;
    protected Button BackStepButton;
    protected Button ToggleStateButton;
    protected Button ForwardStepButton;
    protected Button ForwardLeapButton;
    private Toggle colorToggle;
    protected Toggle ColorToggle
    {
        get
        {
            if (colorToggle == null)
            {
                colorToggle = Root.Q<Toggle>("Color-Toggle");
            }
            return colorToggle;
        }
    }

    private Button tickAgentButton;
    protected Button TickAgentButton
    {
        get
        {
            if (tickAgentButton == null)
            {
                tickAgentButton = Root.Query<Button>(ConstsEditor.Button_TickAgent_Name);
            }
            return tickAgentButton;
        }
    }

    protected SliderInt TickSlider;

    protected Toggle RecordToggle;

    protected VisualElement Body;

    private HelpBox helpBox;
    protected HelpBox HelpBox
    {
        get
        {
            if (helpBox == null)
            {
                helpBox = Root.Query<HelpBox>().First();
            }
            return helpBox;
        }
    }

    private AgentLogComponent agentLogComponent;
    protected AgentLogComponent AgentLogComponent
    {
        get
        {
            if (agentLogComponent == null)
            {
                agentLogComponent = Root.Query<AgentLogComponent>().First();
            }
            return agentLogComponent;
        }
    }

    public IAgent Agent { get; protected set; }
    protected Ai PlayAi;

    protected int CurrentTick
    {
        get => LoggerComponent.CurrentTick;
        set => LoggerComponent.CurrentTick = value;
    }

    protected LoggerState(TemplateContainer root, LoggerComponent debuggerComponent)
    {
        LoggerComponent = debuggerComponent;
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

    protected virtual void SetCurrentTick(int tick)
    {
        CurrentTick = tick;
        if (CurrentTick > TickSlider.highValue)
        {
            TickSlider.highValue = CurrentTick;
        }
        TickSlider.SetValueWithoutNotify(tick);
        TickSlider.label = tick.ToString();
        InspectAi(tick);
    }

    internal virtual void UpdateUi(IAgent agent)
    {
        Agent = agent;
        if (agent == null)
        {
        }
        else
        {
            AgentLogComponent.style.display = DisplayStyle.Flex;
            var log = AiLoggerService.Instance.GetAiDebugLog(Agent, CurrentTick);
            AgentLogComponent.UpdateUi(log);
        }
    }

    internal virtual void OnEnter(IAgent agent) { 
        UpdateUi(agent);
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
    }

    internal virtual void RecordToggleChanged(bool value)
    {

    }

    internal virtual void KeyPressed(KeyDownEvent key)
    {

    }

    protected void InspectAi(int tick)
    {
        var agentLog = AiLoggerService.Instance.GetAiDebugLog(Agent, tick);

        if (agentLog == null)
        {
            AgentLogComponent.Hide();
            HelpBox.style.display = DisplayStyle.Flex;
        }
        else
        {
            HelpBox.style.display = DisplayStyle.None;
            AgentLogComponent.UpdateUi(agentLog);
            if (ColorToggle.value)
            {
                AgentLogComponent.SetColor();
            } else
            {
                AgentLogComponent.ResetColor();
            }
        }
    }
}