using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UniRx;

internal class DebuggerComponent : VisualElement
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private AiTicker aiTicker;
    private AiDebuggerService aiDebuggerService;

    private Label infoLabelLeft;

    private Button backLeapButton;
    private Button backStepButton;
    private Button toggleStateButton;
    private Button forwardStepButton;
    private Button forwardLeapButton;

    private Label tickCount;
    private Slider tickSlider;

    private Toggle recordToggle;

    private VisualElement body;


    private AgentComponent agentComponent;
    private IAgent agent;
    //private UAIComponent aiComponent;
    private Ai displayedAi;
    private Ai agentPlayAi;

    private int currentTick = 0;
    private int stepSize = 1;
    private int leapSize = 10;
    private bool isPlaying => EditorApplication.isPlaying;
    private bool isPaused => EditorApplication.isPaused;

    public DebuggerComponent(IAgent agent)
    {
        this.agent = agent;
        this.aiTicker = AiTicker.Instance;
        this.aiDebuggerService = AiDebuggerService.Instance;
        this.displayedAi = agent?.Ai;
        
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        infoLabelLeft = root.Q<Label>("InfoLeft-Label");

        backLeapButton = root.Q<Button>("BackLeapButton");
        backStepButton = root.Q<Button>("BackStepButton");
        toggleStateButton = root.Q<Button>("ToggleButton");
        forwardStepButton = root.Q<Button>("ForwardStepButton");
        forwardLeapButton = root.Q<Button>("ForwardLeapButton");

        tickCount = root.Q<Label>("TickCount-Label");
        tickSlider = root.Q<Slider>("Tick-Slider");

        recordToggle = root.Q<Toggle>("Record-Toggle");

        body = root.Q<VisualElement>("Body");

        //this.agentComponent = new AgentComponent(agent);
        //body.Add(this.agentComponent);

        infoLabelLeft.text = Consts.Label_DebuggerText;

        backLeapButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            InspectTick(currentTick-leapSize);
            UpdateUI();
        });

        backStepButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            InspectTick(currentTick - stepSize);
            UpdateUI();
        });

        toggleStateButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (isPlaying)
            {
                EditorApplication.isPaused = !isPaused;
            } else
            {
                EditorApplication.isPlaying = true;
            }
        });

        forwardStepButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            InspectTick(currentTick + stepSize);
            UpdateUI();
        });

        forwardLeapButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            InspectTick(currentTick + leapSize);
            UpdateUI();
        });

        tickSlider.value = currentTick;
        tickSlider.RegisterCallback<ChangeEvent<int>>(evt =>
        {
            currentTick = evt.newValue;
            UpdateUI();
        });

        EditorApplication.pauseStateChanged += _ => GameStateChanged();

        aiTicker
            .OnTickComplete
            .Subscribe(tickCount =>
            {
                TickChanged(tickCount);
            })
            .AddTo(disposables);
        UpdateAgent(agent);
        GameStateChanged();
        //UpdateUI();
    }

    private void InspectTick(int tickToInspect)
    {
        Debug.Break();
        currentTick = tickToInspect;
        var aiAtTick = aiDebuggerService.GetAiAtTick(agent, tickToInspect);
        if (aiAtTick == null) 
        {
            Debug.Log("Dispaly something like: Nothing recorded at current tick");
            return;
        }

        //UpdateAi(aiAtTick);
        UpdateUI();
    }

    private void TickChanged(int tickCount)
    {
        if (recordToggle.value)
        {
            if (tickCount == agent.Ai.Context.GetContext<TickMetaData>(AiContextKey.TickMetaData).TickCount)
            {
                aiDebuggerService.AddTick(agent, agent.Ai, tickCount);
            }
        }
        UpdateUI();
    }

    //private void UpdateAi(Ai ai)
    //{
    //    this.displayedAi = ai;
    //    agent.Ai = this.displayedAi;
    //    agentComponent.UpdateAiComponent();
    //    UpdateUI();
    //}

    internal void UpdateAgent(IAgent agent)
    {
        this.agent = agent;
        if (agent == null)
        {
            agentComponent = null;
            body.Clear();
        } else
        {
            if (agentComponent == null)
            {
                body.Clear();
                agentComponent = new AgentComponent(agent);
                body.Add(agentComponent);
            }
            else
            {
                agentComponent.UpdateAgent(agent);
            }
        }
        
        UpdateUI();

    }

    private void GameStateChanged()
    {
        if (isPlaying && !isPaused)
        {
            GameRunning();
        }
        else if (isPlaying && isPaused)
        {
            GamePaused();
        }
    }

    private void GameRunning()
    {
        toggleStateButton.text = "Pause";
        this.displayedAi = agent?.Ai;
        UpdateAgent(agent);
        //UpdateAi(agentPlayAi);
        //if (agentComponent == null)
        //{
        //    UpdateAgent(agent);
        //} else
        //{
        //    agentComponent.UpdateAgent(agent);
        //}
        //UpdateUI();
    }

    private void GamePaused()
    {
        toggleStateButton.text = "Play";
        agentPlayAi = agent?.Ai;

        //if (displayedAi != null)
        //{
        //    UpdateAi(displayedAi);
        //}

        //UpdateUI();
    }

    private void UpdateUI()
    {
        tickSlider.lowValue = aiDebuggerService.MinTick;
        tickSlider.highValue = aiDebuggerService.MaxTick;
        tickSlider.value = aiDebuggerService.MaxTick;
        if (agent == null) return;

        var metaData = agent.Ai.Context.GetContext<TickMetaData>(AiContextKey.TickMetaData);
        if (metaData != null)
        {
            if (metaData.TickCount == AiTicker.Instance.TickCount)
            {
                currentTick = AiTicker.Instance.TickCount;
            }
        }
        

        tickCount.text = currentTick.ToString();
    }

    ~DebuggerComponent()
    {
        disposables.Clear();
    }
}
