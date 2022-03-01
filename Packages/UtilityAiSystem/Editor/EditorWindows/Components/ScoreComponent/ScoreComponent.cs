using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;

public class ScoreComponent: VisualElement
{
    private CompositeDisposable subscriptions = new CompositeDisposable();

    private TemplateContainer root;
    private Label scoreName;
    private Label score;

    public ScoreComponent(ScoreModel model)
    {
        var path = AssetDatabaseService.GetAssetPath(GetType().ToString(), "uxm");
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        root = template.CloneTree();
        Add(root);

        scoreName = root.Q<Label>("Name");
        this.score = root.Q<Label>("Score");

        Setname(model.Name);
        SetScore(model.Value);

        model.OnValueChanged
            .Subscribe(value => UpdateScore(value))
            .AddTo(subscriptions);
    }

    public void UpdateScore(float score, string name = null)
    {
        if (name != null)
        {
            Setname(name);
        }
        SetScore(score);
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
