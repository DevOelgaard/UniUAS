using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

public class MainWindowFoldedComponent : VisualElement
{
    private CompositeDisposable subscriptions = new CompositeDisposable();

    private Label nameLabel;
    private Label descriptionLabel;
    private VisualElement scoreContainer;
    private ScoreComponent baseScore;
    private MainWindowModel model;

    public MainWindowFoldedComponent(MainWindowModel model)
    {
        //var path = AssetDatabaseService.GetAssetPath(typeof(MainWindowFoldedComponent).FullName, "uxml");
        //template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        //var root = template.CloneTree();
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        Add(root);

        nameLabel = this.Q<VisualElement>("NameIdentifier").Q<Label>("Value-Label");
        descriptionLabel = this.Q<VisualElement>("DescriptionIdentifier").Q<Label>("Value-Label");
        scoreContainer = this.Q<VisualElement>("ScoreContainer");

        UpdateContent(model);
    }

    private void UpdateContent(MainWindowModel model)
    {
        subscriptions.Clear();
        this.model = model;

        nameLabel.text = model.Name;
        descriptionLabel.text = model.Description;

        model.OnNameChanged
            .Subscribe(name => nameLabel.text = name)
            .AddTo(subscriptions);

        model.OnDescriptionChanged
            .Subscribe(description => descriptionLabel.text = description)
            .AddTo(subscriptions);

        scoreContainer.Clear();
        foreach(var scoreModel in model.ScoreModels)
        {
            var scoreComponent = new ScoreComponent(scoreModel);
            scoreContainer.Add(scoreComponent);
        }

        //if (model.GetType().IsAssignableFrom(typeof(UtilityContainer)))
        //{
        //    Debug.Log("Adding score");
        //    var modelCast = model as UtilityContainer;
        //    baseScore = new ScoreComponent("Base", modelCast.BaseScore);

        //    scoreContainer.Add(baseScore);

        //    modelCast.BaseScoreChanged
        //        .Subscribe(score => baseScore.UpdateScore(score))
        //        .AddTo(subscriptions);
        //}

        //if (model.GetType().IsSubclassOf(typeof(Consideration)))
        //{
        //    Debug.Log("Adding score");
        //    var modelCast = model as Consideration;
        //    baseScore = new ScoreComponent("Base", modelCast.BaseScore);

        //    scoreContainer.Add(baseScore);

        //    modelCast.BaseScoreChanged
        //        .Subscribe(score => baseScore.UpdateScore(score))
        //        .AddTo(subscriptions);
        //}
    }

    ~MainWindowFoldedComponent()
    {
        subscriptions.Clear();
    }
}