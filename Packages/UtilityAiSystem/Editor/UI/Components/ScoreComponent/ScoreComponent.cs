﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

public class ScoreComponent: VisualElement
{
    private CompositeDisposable subscriptions = new CompositeDisposable();

    private TemplateContainer root;
    private Label scoreName;
    private Label score;
    private VisualElement colorContainer;
    private Color defaultColor;
    private bool dynamicColor;

    public ScoreComponent(ScoreModel model, bool dynamicColor = false)
    {
        this.dynamicColor = dynamicColor;
        var path = AssetDatabaseService.GetAssetPath(GetType().ToString(), "uxm");
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        root = template.CloneTree();
        Add(root);

        scoreName = root.Q<Label>("Name");
        this.score = root.Q<Label>("Score");
        colorContainer = root.Q<VisualElement>("ColorContainer");

        Setname(model.Name);
        SetScore(model.Value);


        model.OnValueChanged
            .Subscribe(value => UpdateScore(value))
            .AddTo(subscriptions);

        defaultColor = new Color(0,0,0,0);
    }

    public void UpdateScore(float score, string name = null)
    {
        if (name != null)
        {
            Setname(name);
        }
        SetScore(score);
        if (dynamicColor)
        {
            SetColor(score);
        }
    }

    internal void SetColor(float percentOfMax)
    {
        percentOfMax = Mathf.Clamp(percentOfMax, 0f, 1f);
        colorContainer.style.backgroundColor = new Color(1 - percentOfMax, percentOfMax, 0, 1);
    }

    internal void SetDefualtColor()
    {
        colorContainer.style.backgroundColor = defaultColor;

    }

    private void SetScore(float score)
    {
        this.score.text = score.ToString("F2");
    }

    private void Setname(string name)
    {
        scoreName.text = name + ": ";
    }

    ~ScoreComponent()
    {
        subscriptions.Clear();
    }
}
