using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UniRx;
using System;

internal abstract class MainWindowComponent: VisualElement
{
    protected CompositeDisposable Subscriptions = new CompositeDisposable();

    private TextField nameTextField;
    private TextField descriptionTextField;
    protected MainWindowModel MainWindowModel;
    protected VisualElement ScoreContainer;
    protected VisualElement Body;
    protected VisualElement Footer;
    protected InfoComponent InfoComponent;

    internal List<ScoreComponent> ScoreComponents = new List<ScoreComponent>();

    protected MainWindowComponent(MainWindowModel mainWindowModel)
    {
        var root = AssetDatabaseService.GetTemplateContainer(typeof(MainWindowComponent).FullName);
        Add(root);
        nameTextField = this.Q<TextField>("Name-TextField");
        nameTextField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (MainWindowModel != null)
            {
                MainWindowModel.Name = evt.newValue;
            }
        });

        descriptionTextField = root.Q<TextField>("Description-TextField");
        descriptionTextField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (MainWindowModel != null)
            {
                MainWindowModel.Description = evt.newValue;
            }
        });

        ScoreContainer = this.Q<VisualElement>("Scores");
        Body = this.Q<VisualElement>("Body");
        Footer = this.Q<VisualElement>("Footer");

        SetFooter();

        MainWindowModel = mainWindowModel;
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
    }


    protected void ClearScore()
    {
        ScoreContainer.Clear();
    }

    protected virtual void SetFooter()
    {
        InfoComponent = new InfoComponent();
        Footer.Add(InfoComponent);
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
        Subscriptions?.Clear();
    }
}