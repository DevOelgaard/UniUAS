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

    private AiTicker aiTicker;

    [MenuItem(Consts.MenuName + Consts.Name_AiTickerManager)]
    public static void Open()
    {
        AiTickerManagerWindow wnd = GetWindow<AiTickerManagerWindow>();
        wnd.titleContent = new GUIContent(Consts.Name_AiTickerManager);
        wnd.Show();
        wnd.position = new Rect(0f, 0f, 1920 / 3, 1080 / 2);
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);

        modes = root.Q<EnumField>("Mode-EnumField");
        header = root.Q<VisualElement>("Header");
        body = root.Q<VisualElement>("Body");
        footer = root.Q<VisualElement>("Footer");

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

        LoadTicker(aiTicker.Settings.TickerMode);

    }

    private void LoadTicker(TickerMode tickerMode)
    {
        body.Clear();
        description.text = "";
        if (tickerMode == null) return;

        tickerMode.Parameters
            .ForEach(p =>
            {
                var parameterComponent = new ParameterComponent(p);
                body.Add(parameterComponent);
            });

        description.text = tickerMode.Description;
    }
}