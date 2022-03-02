using System;
using System.Collections.Generic;
using UniRx;
using UniRxExtension;

[Serializable]
public abstract class UtilityContainer : AiObjectModel
{
    private IDisposable considerationSub;
    private ReactiveList<Consideration> considerations;
    internal ReactiveList<Consideration> Considerations
    {
        get { 
            if (considerations == null)
            {
                considerations = new ReactiveList<Consideration>();
            }
            return considerations;
        }
        set
        {
            considerations = value;
            if (considerations != null)
            {
                considerationSub?.Dispose();
                UpdateInfo();
                considerationSub = considerations.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }

    public float LastCalculatedUtility { get; protected set; } = -1f;
    public IObservable<float> LastUtilityScoreChanged => lastUtilityChanged;
    private Subject<float> lastUtilityChanged = new Subject<float>();

    protected UtilityContainer() : base()
    {
        ScoreModels = new List<ScoreModel>();
        ScoreModels.Add(new ScoreModel("Base", 0f));
        ScoreModels.Add(new ScoreModel("Normalized", 0f));
    }

    internal virtual float GetUtility(AiContext context)
    {
        LastCalculatedUtility = context.UtilityScorer.CalculateUtility(Considerations.Values, context);
        lastUtilityChanged.OnNext(LastCalculatedUtility);
        return LastCalculatedUtility;
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        considerationSub?.Dispose();
    }


}