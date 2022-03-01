using System;
using System.Collections.Generic;
using UniRx;
using UniRxExtension;

[Serializable]
public abstract class UtilityContainer : MainWindowModel
{
    public ReactiveList<Consideration> Considerations = new ReactiveList<Consideration>();

    public float LastCalculatedUtility { get; protected set; } = -1f;
    public IObservable<float> LastUtilityScoreChanged => lastUtilityChanged;
    private Subject<float> lastUtilityChanged = new Subject<float>();

    protected UtilityContainer() : base()
    {
        ScoreModels = new List<ScoreModel>();
        ScoreModels.Add(new ScoreModel("Base", 0f));
        ScoreModels.Add(new ScoreModel("Normalized", 0f));
    }

    public virtual float GetUtility(AiContext context)
    {
        LastCalculatedUtility = context.UtilityScorer.CalculateUtility(Considerations.Values, context);
        lastUtilityChanged.OnNext(LastCalculatedUtility);
        return LastCalculatedUtility;
    }

}