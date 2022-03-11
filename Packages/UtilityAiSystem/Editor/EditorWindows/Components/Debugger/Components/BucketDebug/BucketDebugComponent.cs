using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class BucketDebugComponent : AiObjectDebugComponent
{
    private VisualElement considerationsContainer;
    private VisualElement decisionsContainr;

    public BucketDebugComponent(BucketDebug b) : base(b)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        decisionsContainr = root.Q<VisualElement>("DecisionsContainer");

        var score = new ScoreComponent(new ScoreModel("Score", b.Score));
        ScoreContainer.Add(score);

        var weight = new ParameterDebugComponent(new KeyValuePair<string, string>("Weight", b.Weight.ToString()));
        ScoreContainer.Add(weight);

        foreach(var c in b.Considerations)
        {
            considerationsContainer.Add(new ConsiderationDebugComponent(c));
        }

        foreach(var d in b.Decisions)
        {
            decisionsContainr.Add(new DecisionDebugComponent(d));
        }
    }
}
