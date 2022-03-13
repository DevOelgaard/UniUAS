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
    protected CompositeDisposable modelInfoChangedDisposable = new CompositeDisposable();

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

    protected MainWindowComponent()
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

        SetFooter();
    }

    internal void UpdateUi(AiObjectModel model)
    {
        Model = model;
        nameTextField.value = model.Name;
        descriptionTextField.value = model.Description;
        
        ScoreContainer.Clear();
        ScoreComponents = new List<ScoreComponent>();
        foreach (var scoreModel in model.ScoreModels)
        {
            var scoreComponent = new ScoreComponent(scoreModel);
            ScoreComponents.Add(scoreComponent);
            ScoreContainer.Add(scoreComponent);
        }

        modelInfoChangedDisposable.Clear();
        Model.OnInfoChanged
            .Subscribe(info => InfoComponent.DispalyInfo(info))
            .AddTo(modelInfoChangedDisposable);
        InfoComponent.DispalyInfo(Model.Info);
        UpdateInternal(model);
    }

    protected abstract void UpdateInternal(AiObjectModel model);

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
        modelInfoChangedDisposable?.Clear();
    }
}