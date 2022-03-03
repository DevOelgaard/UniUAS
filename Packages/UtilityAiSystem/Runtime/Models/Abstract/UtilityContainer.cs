using System;
using System.Collections.Generic;
using UniRx;
using UniRxExtension;

[Serializable]
public abstract class UtilityContainer : AiObjectModel
{
    private IDisposable considerationSub;
    private ReactiveList<Consideration> considerations = new ReactiveList<Consideration>();
    internal ReactiveList<Consideration> Considerations
    {
        get => considerations;
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

    private float lastCalculatedUtility;
    public float LastCalculatedUtility
    {
        get => lastCalculatedUtility;
        set
        {
            lastCalculatedUtility = value;
            ScoreModels[0].Value = value;
            lastUtilityChanged.OnNext(value);
        }
    }
    
    public IObservable<float> LastUtilityScoreChanged => lastUtilityChanged;
    private Subject<float> lastUtilityChanged = new Subject<float>();

    protected UtilityContainer() : base()
    {
        ScoreModels = new List<ScoreModel>();
        ScoreModels.Add(new ScoreModel("Score", 0f));
        //ScoreModels.Add(new ScoreModel("Normalized", 0f));

        considerationSub?.Dispose();
        UpdateInfo();
        considerationSub = considerations.OnValueChanged
            .Subscribe(_ => UpdateInfo());
    }

    internal virtual float GetUtility(AiContext context)
    {
        LastCalculatedUtility = context.UtilityScorer.CalculateUtility(Considerations.Values, context);
        return LastCalculatedUtility;
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        considerationSub?.Dispose();
    }


}