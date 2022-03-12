﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ResponseFunctionLogComponent: LogComponent
{
    private Label typeLabel;
    private VisualElement body;
    //private ParameterLogComponentPool pool;
    private LogComponentPool<ParameterLogComponent> pool;
    public ResponseFunctionLogComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        typeLabel = root.Q<Label>("Type-Label");
        body = root.Q<VisualElement>("Body");
        pool = new LogComponentPool<ParameterLogComponent>(body);
    }

    internal override void Display(ILogModel element)
    {
        var rf = element as ResponseFunctionLog;
        typeLabel.text = rf.Type.ToString();
        var logModels = new List<ILogModel>();
        foreach(var p in rf.Parameters)
        {
            logModels.Add(p);
        }
        pool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        pool.Hide();
    }
}