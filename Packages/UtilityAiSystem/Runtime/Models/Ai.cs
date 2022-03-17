using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;

public class Ai: AiObjectModel
{
    private IDisposable bucketSub;
    public bool IsPLayable = false;
    private ReactiveListNameSafe<Bucket> buckets = new ReactiveListNameSafe<Bucket>();
    public ReactiveListNameSafe<Bucket> Buckets
    {
        get => buckets;
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

    public Ai(): base()
    {
        bucketSub?.Dispose();
        UpdateInfo();
        bucketSub = buckets.OnValueChanged
            .Subscribe(_ => UpdateInfo());
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

    internal override RestoreState GetState()
    {
        return new UAIModelState(Name, Description, Buckets.Values, this);
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Ai>(state);
        return clone;
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (UAIModelState)s;
        Name = state.Name;
        Description = state.Description;
        IsPLayable = state.IsPLayable;

        Buckets = new ReactiveListNameSafe<Bucket>();
        foreach(var bS in state.Buckets)
        {
            var b = Bucket.Restore<Bucket>(bS, restoreDebug);
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

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
    private UtilityContainerSelector bucketSelector;
    public UtilityContainerSelector BucketSelector
    {
        get
        {
            if (bucketSelector == null)
            {
                bucketSelector = ScorerService.Instance.ContainerSelectors.Values
                    .FirstOrDefault(bS => bS.GetName() == Consts.Default_BucketSelector);
            }
            return bucketSelector;
        }
        set
        {
            bucketSelector = value;
        }
    }

    private UtilityContainerSelector decisionSelector;
    public UtilityContainerSelector DecisionSelector
    {
        get {
            if (decisionSelector == null)
            {
                decisionSelector = ScorerService.Instance.ContainerSelectors.Values
                    .FirstOrDefault(bS => bS.GetName() == Consts.Default_DecisionSelector);
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
                    .FirstOrDefault(e => e.GetName() == Consts.Default_UtilityScorer);
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
    public bool IsPLayable;
    public List<BucketState> Buckets = new List<BucketState>();
    public string BucketSelectorName;
    public string DecisionSelectorName;
    public string USName;

    public UAIModelState() : base()
    {
    }

    public UAIModelState(string name, string description, List<Bucket> buckets, Ai model): base(model)
    {
        Name = name;
        Description = description;
        IsPLayable = model.IsPLayable;
        Buckets = new List<BucketState>();
        foreach(var b in buckets)
        {
            var bS = b.GetState() as BucketState;
            Buckets.Add(bS);
        }

        BucketSelectorName = model.BucketSelector.GetName();
        DecisionSelectorName = model.DecisionSelector.GetName();
        USName = model.UtilityScorer.GetName();
    }
}