﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UniRx;
using System;

internal abstract class MainWindowComponent: VisualElement
{
    protected CompositeDisposable Disposables = new CompositeDisposable();

    private TextField nameTextField;
    private TextField descriptionTextField;
    protected AiObjectModel Model;
    protected VisualElement ScoreContainer;
    protected VisualElement Header;
    protected VisualElement Body;
    protected VisualElement Footer;
    protected InfoComponent InfoComponent;
    protected Button SaveToTemplate;

    internal List<ScoreComponent> ScoreComponents = new List<ScoreComponent>();

    protected MainWindowComponent(AiObjectModel mainWindowModel)
    {
        var root = AssetDatabaseService.GetTemplateContainer(typeof(MainWindowComponent).FullName);
        Add(root);
        nameTextField = this.Q<TextField>("Name-TextField");
        nameTextField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (Model != null)
            {
                Model.Name = evt.newValue;
            }
        });

        descriptionTextField = root.Q<TextField>("Description-TextField");
        descriptionTextField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (Model != null)
            {
                Model.Description = evt.newValue;
            }
        });

        ScoreContainer = this.Q<VisualElement>("Scores");
        Header = this.Q<VisualElement>("Header");
        Body = this.Q<VisualElement>("Body");
        Footer = this.Q<VisualElement>("Footer");
        SaveToTemplate = this.Q<Button>("SaveToTemplate-Button");
        SaveToTemplate.RegisterCallback<MouseUpEvent>(evt =>
        {
            var clone = Model.Clone();
            UASTemplateService.Instance.Add(clone);
        });


        Model = mainWindowModel;
        ScoreContainer.Clear();

        nameTextField.value = mainWindowModel.Name;
        descriptionTextField.value = mainWindowModel.Description;

        ScoreComponents = new List<ScoreComponent>();
        foreach (var scoreModel in mainWindowModel.ScoreModels)
        {
            var scoreComponent = new ScoreComponent(scoreModel);
            ScoreComponents.Add(scoreComponent);
            ScoreContainer.Add(scoreComponent);
        }

        SetFooter();

    }


    protected void ClearScore()
    {
        ScoreContainer.Clear();
    }

    protected virtual void SetFooter()
    {
        InfoComponent = new InfoComponent();
        Footer.Add(InfoComponent);
        InfoComponent.DispalyInfo(Model.Info);

        Model.OnInfoChanged
            .Subscribe(info => InfoComponent.DispalyInfo(info))
            .AddTo(Disposables);
    }

    internal void Close()
    {
        AssetDatabase.SaveAssets();
    }

    ~MainWindowComponent()
    {
        ClearSubscriptions();
    }

    protected virtual void ClearSubscriptions()
    {
        Disposables?.Clear();
    }
}