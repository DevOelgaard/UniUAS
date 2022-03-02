using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;

public class Bucket : UtilityContainer 
{
    private IDisposable decisionSub;
    private ReactiveList<Decision> decisions = new ReactiveList<Decision>();
    public ReactiveList<Decision> Decisions
    {
        get => decisions;
        set
        {
            decisions = value;
            if (decisions != null)
            {
                decisionSub?.Dispose();
                UpdateInfo();
                decisionSub = decisions.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }
    public float Weight = 1;

    public Bucket(): base()
    {

    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Bucket>(state);
        return clone;
    }

    internal override float GetUtility(AiContext context)
    {
        LastCalculatedUtility = base.GetUtility(context) * Weight;
        return LastCalculatedUtility;
    }

    protected override void UpdateInfo()
    {
        base.UpdateInfo();
        if (Decisions.Count <= 0)
        {
            Info = new InfoModel("No Decisions, Object won't be selected",InfoTypes.Warning);
        } else if (Considerations.Count <= 0)
        {
            Info = new InfoModel("No Considerations, Object won't be selected", InfoTypes.Warning);
        } else
        {
            Info = new InfoModel();
        }
    }

    internal BucketState GetState()
    {
        return new BucketState(Name, Description, Decisions.Values, Considerations.Values, this);
    }
    protected override void RestoreInternal(RestoreState state)
    {
        var stateCast = (BucketState)state;
        Name = stateCast.Name;
        Description = stateCast.Description;

        Decisions = new ReactiveList<Decision>();
        foreach (var d in stateCast.Decisions)
        {
            var decision = Restore<Decision>(d);
            Decisions.Add(decision);
        }

        Considerations = new ReactiveList<Consideration>();
        foreach (var c in stateCast.Considerations)
        {
            var consideration = Restore<Consideration>(c);
            Considerations.Add(consideration);
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        decisionSub?.Dispose();
    }
}

[Serializable]
public class BucketState: RestoreState
{
    public string Name;
    public string Description;
    public List<DecisionState> Decisions;
    public List<ConsiderationState> Considerations;

    public BucketState(): base()
    {
    }

    public BucketState(string name, string description, List<Decision> decisions, List<Consideration> considerations, RestoreAble o) : base(o)
    {
        this.Name = name;
        this.Description = description;
        this.Decisions = new List<DecisionState>();
        foreach (var d in decisions)
        {
            var state = d.GetState();
            Decisions.Add(state);
        }

        this.Considerations = new List<ConsiderationState>();
        foreach (var c in considerations)
        {
            var state = c.GetState();
            Considerations.Add(state);
        }
    }
}