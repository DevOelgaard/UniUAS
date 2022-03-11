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
    private UAIComponent aiComponent;
    private Ai displayedAi;

    private int currentTick = 0;
    private int stepSize = 1;
    private int leapSize = 10;
    private bool isPlaying => EditorApplication.isPlaying;


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
                Debug.Break();
            }
            else
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
            .OnTickCountChanged
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

        this.displayedAi = aiAtTick;
        body.Clear();
        this.aiComponent = new UAIComponent(this.displayedAi);
        body.Add(this.aiComponent);
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

    internal void UpdateAgent(IAgent agent)
    {
        body.Clear();
        if (agent != null)
        {
            this.agent = agent;
            this.agentComponent = new AgentComponent(agent);
            body.Add(this.agentComponent);
            this.displayedAi = agent.Ai;
        }

        UpdateUI();
    }

    private void GameStateChanged()
    {
        if (isPlaying)
        {
            GameRunning();
        }
        else
        {
            GamePaused();
        }
    }

    private void GameRunning()
    {
        toggleStateButton.text = "Pause";
        this.displayedAi = agent?.Ai;
        if (agentComponent == null)
        {
            UpdateAgent(agent);
        } else
        {
            agentComponent.UpdateAgent(agent);
        }
        UpdateUI();
    }

    private void GamePaused()
    {
        toggleStateButton.text = "Play";
        body.Clear();
        body.Add(aiComponent);
        aiComponent.UpdateAi(displayedAi);

        UpdateUI();
    }

    private void UpdateUI()
    {
        tickCount.text = currentTick.ToString();
        tickSlider.lowValue = aiDebuggerService.MinTick;
        tickSlider.highValue = aiDebuggerService.MaxTick;
        tickSlider.value = aiDebuggerService.MaxTick;
    }

    ~DebuggerComponent()
    {
        disposables.Clear();
    }
}
