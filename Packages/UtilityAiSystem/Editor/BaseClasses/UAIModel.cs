using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;

public class UAIModel: AiObjectModel
{
    private IDisposable bucketSub;
    private ReactiveList<Bucket> buckets;
    public ReactiveList<Bucket> Buckets
    {
        get
        {
            if (buckets == null)
            {
                buckets = new ReactiveList<Bucket>();
            }
            return buckets;
        }
        set
        {
            buckets = value;
            if (buckets != null)
            {
                bucketSub?.Dispose();
                UpdateInfo();
                bucketSub = buckets.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }
    internal AiContext Context = new AiContext();

    public UAIModel(): base()
    {
    }

    protected override void UpdateInfo()
    {
        
        base.UpdateInfo();
        if (Buckets == null || Buckets.Count <= 0)
        {
            Info = new InfoModel("No Buckets, Object won't be selected", InfoTypes.Warning);
        }
        else
        {
            Info = new InfoModel();
        }
    }

    internal UAIModelState GetState()
    {
        return new UAIModelState(Name, Description, Buckets.Values, this);
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<UAIModel>(state);
        return clone;
    }

    protected override void RestoreInternal(RestoreState s)
    {
        var state = (UAIModelState)s;
        Name = state.Name;
        Description = state.Description;

        Buckets = new ReactiveList<Bucket>();
        foreach(var bS in state.Buckets)
        {
            var b = Bucket.Restore<Bucket>(bS);
            Buckets.Add(b);
        }
        var scorerService = ScorerService.Instance;

        BucketSelector = scorerService
            .ContainerSelectors
            .Values
            .FirstOrDefault(d => d.GetName() == state.BucketSelectorName);
        if (BucketSelector == null)
        {
            BucketSelector = ScorerService.Instance.ContainerSelectors.Values.FirstOrDefault();
        }

        DecisionSelector = scorerService
            .ContainerSelectors
            .Values
            .FirstOrDefault(d => d.GetName() == state.DecisionSelectorName);
        if (DecisionSelector == null)
        {
            DecisionSelector = ScorerService.Instance.ContainerSelectors.Values.FirstOrDefault();
        }

        UtilityScorer = scorerService
            .UtilityScorers
            .Values
            .FirstOrDefault(u => u.GetName() == state.USName);
        if (UtilityScorer == null)
        {
            UtilityScorer = scorerService.UtilityScorers.Values.FirstOrDefault();
        }
    }

    private IUtilityContainerSelector bucketSelector;
    public IUtilityContainerSelector BucketSelector
    {
        get
        {
            if (bucketSelector == null)
            {
                bucketSelector = ScorerService.Instance.ContainerSelectors.Values
                    .FirstOrDefault(bS => bS.GetName() == Statics.Default_BucketSelector);
            }
            return bucketSelector;
        }
        set
        {
            bucketSelector = value;
        }
    }

    private IUtilityContainerSelector decisionSelector;
    public IUtilityContainerSelector DecisionSelector
    {
        get {
            if (decisionSelector == null)
            {
                decisionSelector = ScorerService.Instance.ContainerSelectors.Values
                    .FirstOrDefault(bS => bS.GetName() == Statics.Default_DecisionSelector);
            }
            return decisionSelector;
        }
        set
        {
            decisionSelector = value;
        }
    }

    private IUtilityScorer utilityScorer;
    internal IUtilityScorer UtilityScorer
    {
        get
        {
            if (utilityScorer == null)
            {
                utilityScorer = ScorerService
                    .Instance
                    .UtilityScorers
                    .Values
                    .FirstOrDefault(e => e.GetName() == Statics.Default_UtilityScorer);
            }
            return utilityScorer;
        }
        set
        {
            utilityScorer = value;
            Context.UtilityScorer = UtilityScorer;
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        bucketSub?.Dispose();
    }
}

public class UAIModelState: RestoreState
{
    public string Name;
    public string Description;
    public List<BucketState> Buckets = new List<BucketState>();
    public string BucketSelectorName;
    public string DecisionSelectorName;
    public string USName;

    public UAIModelState() : base()
    {
    }

    public UAIModelState(string name, string description, List<Bucket> buckets, UAIModel model): base(model)
    {
        Name = name;
        Description = description;
        Buckets = new List<BucketState>();
        foreach(var b in buckets)
        {
            var bS = b.GetState();
            Buckets.Add(bS);
        }

        BucketSelectorName = model.BucketSelector.GetName();
        DecisionSelectorName = model.DecisionSelector.GetName();
        USName = model.UtilityScorer.GetName();
    }
}