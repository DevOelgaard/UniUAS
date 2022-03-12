﻿using System;
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
    private TemplateContainer root;
    private DebuggerState state;

    private Button backLeapButton;
    private Button backStepButton;
    private Button toggleStateButton;
    private Button forwardStepButton;
    private Button forwardLeapButton;
    private DebuggerGameRunning gameRunning;
    private DebuggerGamePaused gamePaused;
    private DebuggerGameStopped gameStopped;

    private SliderInt tickSlider;

    private bool isPlaying => EditorApplication.isPlaying;
    private bool isPaused => EditorApplication.isPaused;
    private IAgent agent => state?.Agent;

    public DebuggerComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        backLeapButton = root.Q<Button>("BackLeapButton");
        backStepButton = root.Q<Button>("BackStepButton");
        toggleStateButton = root.Q<Button>("ToggleButton");
        forwardStepButton = root.Q<Button>("ForwardStepButton");
        forwardLeapButton = root.Q<Button>("ForwardLeapButton");

        tickSlider = root.Q<SliderInt>("Tick-Slider");

        gameRunning = new DebuggerGameRunning(root, this);
        gamePaused = new DebuggerGamePaused(root, this);
        gameStopped = new DebuggerGameStopped(root, this);

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

    internal void UpdateAgent(IAgent agent)
    {
        state.UpdateAgent(agent);
    }

    private void UpdateGameState()
    {
        if (isPlaying && !isPaused)
        {
            SetState(gameRunning);
        }
        else if (isPlaying && isPaused)
        {
            SetState(gamePaused);
        }
        else
        {
            SetState(gameStopped);
        }
    }

    ~DebuggerComponent()
    {
        disposables.Clear();
    }
}
