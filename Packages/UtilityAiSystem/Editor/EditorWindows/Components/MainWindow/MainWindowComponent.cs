using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UniRx;
using System;

public abstract class MainWindowComponent: VisualElement
{
    protected CompositeDisposable Subscriptions = new CompositeDisposable();

    private VisualTreeAsset template;
    //public TemplateContainer Root { get; private set; }
    private TextField nameTextField;
    private TextField descriptionTextField;
    protected MainWindowModel MainWindowModel;
    protected VisualElement ScoreContainer;
    protected VisualElement Body;
    protected VisualElement Footer;

    public List<ScoreComponent> ScoreComponents = new List<ScoreComponent>();

    protected MainWindowComponent(MainWindowModel mainWindowModel)
    {
        //var path = AssetDatabaseService.GetAssetPath(typeof(MainWindowComponent).FullName, "uxml");
        //template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        //var root = template.CloneTree();
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
        //var deleteButton = new Button(() => MainWindowModel.DeleteObject());
        //deleteButton.text = "Delete File";
        //Footer.Add(deleteButton);

        //var copyButton = new Button(() => MainWindowModel.CopyFile());
        //copyButton.text = "Copy";
        //Footer.Add(copyButton);

        //var saveChangesButton = new Button(() => MainWindowModel.SaveChanges());
        //saveChangesButton.text = "Save Changes";
        //Footer.Add(saveChangesButton);

        //var saveButton = new Button(() => MainWindowModel.SaveFile());
        //saveButton.text = "Save To New Template";
        //Footer.Add(saveButton);
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