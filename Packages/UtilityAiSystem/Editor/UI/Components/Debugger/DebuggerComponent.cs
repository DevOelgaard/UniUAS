using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UniRx;

internal class DebuggerComponent : RightPanelComponent<IAgent>
{
    private CompositeDisposable disposables = new CompositeDisposable();
    
    private TemplateContainer root;
    private VisualElement Body;

    private Button backLeapButton;
    private Button backStepButton;
    private Button toggleStateButton;
    private Button forwardStepButton;
    private Button forwardLeapButton;

    private DebuggerState state;

    private AiLogComponent aiLogComponent;
    private HelpBox helpBox;

    private SliderInt tickSlider;

    private bool isPlaying => EditorApplication.isPlaying;
    private bool isPaused => EditorApplication.isPaused;
    private IAgent agent;
    internal int CurrentTick;

    public DebuggerComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        backLeapButton = root.Q<Button>("BackLeapButton");
        backStepButton = root.Q<Button>("BackStepButton");
        toggleStateButton = root.Q<Button>("ToggleButton");
        forwardStepButton = root.Q<Button>("ForwardStepButton");
        forwardLeapButton = root.Q<Button>("ForwardLeapButton");
        Body = root.Q<VisualElement>("Body");

        tickSlider = root.Q<SliderInt>("Tick-Slider");
        helpBox = new HelpBox();
        helpBox.style.display = DisplayStyle.None;
        Body.Add(helpBox);

        aiLogComponent = new AiLogComponent();
        Body.Add(aiLogComponent);
        aiLogComponent.style.display = DisplayStyle.None;
       

        backLeapButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.BackLeapButtonPressed();
        });

        backStepButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.BackStepButtonPressed();
        });

        toggleStateButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.ToggleStateButtonPressed();
        });

        forwardStepButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.ForwardStepButtonPressed();
        });

        forwardLeapButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.ForwardLeapButtonPressed();
        });
        
        tickSlider.RegisterCallback<ChangeEvent<int>>(evt =>
        {
            state.TickSliderChanged(evt.newValue);
        });
        EditorApplication.pauseStateChanged += _ => UpdateGameState();
        
        UpdateGameState();
    }

    internal void SetState(DebuggerState state)
    {
        this.state?.OnExit();
        this.state = state;
        this.state.OnEnter(agent);
    }

    internal override void UpateUi(IAgent element)
    {
        this.agent = element;
        state.UpdateUi(agent);
    }

    private void UpdateGameState()
    {
        if (isPlaying && !isPaused)
        {
            SetState(new DebuggerGameRunning(root, this));
        }
        else if (isPlaying && isPaused)
        {
            SetState(new DebuggerGamePaused(root, this));
        }
        else
        {
            SetState(new DebuggerGameStopped(root, this));
        }
    }

    ~DebuggerComponent()
    {
        disposables.Clear();
    }
}
