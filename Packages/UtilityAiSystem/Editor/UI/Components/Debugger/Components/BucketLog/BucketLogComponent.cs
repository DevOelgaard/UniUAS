using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class BucketLogComponent : AiObjectLogComponent
{
    private VisualElement considerationsContainer;
    private VisualElement decisionsContainer;
    private LogComponentPool<ConsiderationLogComponent> considerationPool;
    private LogComponentPool<DecisionLogComponent> decisionsPool;
    private ScoreComponent weight;
    private ScoreComponent score;

    public BucketLogComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        decisionsContainer = root.Q<VisualElement>("DecisionsContainer");

        considerationPool = new LogComponentPool<ConsiderationLogComponent>(considerationsContainer,3);
        decisionsPool = new LogComponentPool<DecisionLogComponent>(decisionsContainer,2);

        weight = new ScoreComponent(new ScoreModel("Weight", 0));

        score = new ScoreComponent(new ScoreModel("Score", 0));
        ScoreContainer.Add(score);
    }

    protected override void DisplayInternal(AiObjectLog aiLog)
    {
        var b = aiLog as BucketLog;

        score.UpdateScore(b.Score);

        var logModels = new List<ILogModel>();
        foreach (var c in b.Considerations)
        {
            logModels.Add(c);
        }
        considerationPool.Display(logModels);

        logModels.Clear();
        foreach (var d in b.Decisions)
        {
            logModels.Add(d);
        }
        decisionsPool.Display(logModels);

        weight.UpdateScore(b.Weight);
    }

    internal override void Hide()
    {
        base.Hide();
        considerationPool.Hide();
        decisionsPool.Hide();
    }
}
