using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

internal abstract class LoggerState
{
    protected LoggerComponent DebuggerComponent;
    protected TemplateContainer Root;

    protected Label InfoLabelLeft;

    protected Button BackLeapButton;
    protected Button BackStepButton;
    protected Button ToggleStateButton;
    protected Button ForwardStepButton;
    protected Button ForwardLeapButton;

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

    private AiLogComponent aiLogComponent;
    protected AiLogComponent AiLogComponent
    {
        get
        {
            if (aiLogComponent == null)
            {
                aiLogComponent = Root.Query<AiLogComponent>().First();
            }
            return aiLogComponent;
        }
    }

    public IAgent Agent { get; protected set; }
    protected Ai PlayAi;

    protected int CurrentTick
    {
        get => DebuggerComponent.CurrentTick;
        set => DebuggerComponent.CurrentTick = value;
    }

    protected LoggerState(TemplateContainer root, LoggerComponent debuggerComponent)
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
            AiLogComponent.style.display = DisplayStyle.Flex;
            var log = AiLoggerService.Instance.GetAiDebugLog(Agent, CurrentTick);
            AiLogComponent.UpdateUi(log);
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

    protected void InspectAi(int tick)
    {
        var aiLog = AiLoggerService.Instance.GetAiDebugLog(Agent, tick);

        if (aiLog == null)
        {
            AiLogComponent.Hide();
            HelpBox.style.display = DisplayStyle.Flex;
        }
        else
        {
            HelpBox.style.display = DisplayStyle.None;
            AiLogComponent.UpdateUi(aiLog);
        }
    }
}