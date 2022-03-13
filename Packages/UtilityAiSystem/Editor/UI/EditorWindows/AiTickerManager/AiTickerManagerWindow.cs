using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using MoreLinq;

internal class AiTickerManagerWindow: EditorWindow
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private VisualElement root;
    private EnumField modes;
    private VisualElement header;
    private VisualElement body;
    private VisualElement footer;
    private HelpBox description;
    private Button startButton;
    private Button stopButton;
    private Button reloadButton;
    private Label tickCount;
    private Toggle autoRunToggle;


    private AiTicker aiTicker;

    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);

        modes = root.Q<EnumField>("Mode-EnumField");
        header = root.Q<VisualElement>("Header");
        body = root.Q<VisualElement>("Body");
        footer = root.Q<VisualElement>("Footer");
        startButton = root.Q<Button>("StartButton");
        stopButton = root.Q<Button>("StopButton");
        reloadButton = root.Q<Button>("ReloadButton");
        tickCount = root.Q<Label>("TickCountValue-Label");
        autoRunToggle = root.Q<Toggle>("AutoRun-Toggle");

        description = new HelpBox("",HelpBoxMessageType.Info);
        header.Add(description);
        aiTicker = AiTicker.Instance;
        modes.Init(AiTickerMode.Unrestricted);
        modes.value = aiTicker.Settings.TickerMode.Name;
        modes.RegisterCallback<ChangeEvent<Enum>>(evt =>
        {
            aiTicker.SetTickerMode((AiTickerMode)evt.newValue);
        });

        aiTicker.Settings.OnTickerModeChanged
            .Subscribe(tickerMode => LoadTicker(tickerMode))
            .AddTo(disposables);

        startButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            aiTicker.Start();
        });

        stopButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            aiTicker.Stop();
        });

        reloadButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            aiTicker.Reload();
        });

        LoadTicker(aiTicker.Settings.TickerMode);
        autoRunToggle.value = aiTicker.Settings.AutoRun;
        autoRunToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            aiTicker.Settings.AutoRun = evt.newValue;
        });

        tickCount.text = aiTicker.TickCount.ToString();

        aiTicker.OnTickCountChanged
            .Subscribe(value => tickCount.text = value.ToString())
            .AddTo(disposables);
    }

    private void LoadTicker(TickerMode tickerMode)
    {
        body.Clear();
        description.text = "";
        if (tickerMode == null) return;

        tickerMode.Parameters
            .ForEach(p =>
            {
                var pC = new ParameterComponent();
                pC.UpdateUi(p);
                body.Add(pC);
            });

        description.text = tickerMode.Description;
    }

    private void OnDestroy()
    {
        aiTicker.Save();
        disposables.Clear();
    }

}