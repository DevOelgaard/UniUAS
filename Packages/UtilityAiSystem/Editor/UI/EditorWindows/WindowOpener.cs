using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;

internal class WindowOpener: EditorWindow
{
    [MenuItem(Consts.MenuName + Consts.Name_AiInspector)]
    public static void OpenRuntimInspector()
    {
        RunTimeInspector wnd = GetWindow<RunTimeInspector>();
        wnd.titleContent = new GUIContent(Consts.Name_AiInspector);
        wnd.Show();
        wnd.position = new Rect(0f, 0f, 1920 / 3, 1080 / 2);
    }


    [MenuItem(Consts.MenuName + Consts.Name_AiTickerManager)]
    public static void OpenAiTickerManager()
    {
        AiTickerManagerWindow wnd = GetWindow<AiTickerManagerWindow>();
        wnd.titleContent = new GUIContent(Consts.Name_AiTickerManager);
        wnd.Show();
        wnd.position = new Rect(0f, 0f, 1920 / 3, 1080 / 2);
    }

    [MenuItem(Consts.MenuName + Consts.Name_TemplateManager)]
    internal static void OpenTemplateManager()
    {
        TemplateManager wnd = GetWindow<TemplateManager>();
        wnd.titleContent = new GUIContent(Consts.Name_TemplateManager);
        wnd.Show();
        wnd.position = new Rect(0f, 0f, 1920 / 3, 1080 / 2);
    }
}
