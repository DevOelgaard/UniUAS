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
    private LogComponentPool<ConsiderationLogComponent> considerationsPool;
    private LogComponentPool<DecisionLogComponent> decisionsPool;
    private ScoreLogComponent weight;
    private ScoreLogComponent score;

    public BucketLogComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        decisionsContainer = root.Q<VisualElement>("DecisionsContainer");

        considerationsPool = new LogComponentPool<ConsiderationLogComponent>(considerationsContainer,3);
        decisionsPool = new LogComponentPool<DecisionLogComponent>(decisionsContainer,2);

        weight = new ScoreLogComponent("Weight", 0.ToString());
        ScoreContainer.Add(weight);

        score = new ScoreLogComponent("Score", 0.ToString());
        ScoreContainer.Add(score);
    }

    protected override void UpdateUiInternal(AiObjectLog aiLog)
    {
        var b = aiLog as BucketLog;

        score.UpdateScore(b.Score);

        var logModels = new List<ILogModel>();
        foreach (var c in b.Considerations)
        {
            logModels.Add(c);
        }
        considerationsPool.Display(logModels);

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
        considerationsPool.Hide();
        decisionsPool.Hide();
    }

    internal override void SetColor()
    {
        base.SetColor();

        var list = new List<KeyValuePair<VisualElement, float>>();
        foreach (var c in considerationsPool.LogComponents)
        {
            if (c.Model == null) continue;
            var cast = c.Model as ConsiderationLog;
            list.Add(new KeyValuePair<VisualElement, float>(c, cast.NormalizedScore));
        }
        ColorService.SetColor(list);

        list = new List<KeyValuePair<VisualElement, float>>();
        foreach (var d in decisionsPool.LogComponents)
        {
            if (d.Model == null) continue;
            var cast = d.Model as DecisionLog;
            list.Add(new KeyValuePair<VisualElement, float>(d, cast.Score));
            d.SetColor();
        }
        ColorService.SetColor(list);
    }

    internal override void ResetColor()
    {
        foreach (var c in considerationsPool.LogComponents)
        {
            if (c.Model == null) continue;
            var cast = c.Model as ConsiderationLog;
            c.ResetColor();
        }

        foreach (var d in decisionsPool.LogComponents)
        {
            if (d.Model == null) continue;
            var cast = d.Model as DecisionLog;
            d.ResetColor();
        }
        base.ResetColor();
    }
}
