using UnityEngine;
using UniRx;
using UniRxExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

internal class UASTemplateService: RestoreAble
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private Dictionary<string, ReactiveList<AiObjectModel>> collectionsByLabel = new Dictionary<string, ReactiveList<AiObjectModel>>();

    public ReactiveListNameSafe<AiObjectModel> AIs;
    public ReactiveListNameSafe<AiObjectModel> Buckets;
    public ReactiveListNameSafe<AiObjectModel> Decisions;
    public ReactiveListNameSafe<AiObjectModel> Considerations;
    public ReactiveListNameSafe<AiObjectModel> AgentActions;
    public ReactiveListNameSafe<AiObjectModel> ResponseCurves;

    public IObservable<ReactiveList<AiObjectModel>> OnCollectionChanged => onCollectionChanged;
    private Subject<ReactiveList<AiObjectModel>> onCollectionChanged = new Subject<ReactiveList<AiObjectModel>>();

    private static UASTemplateService instance;

    internal UASTemplateService()
    {
        Init(true);
    }


    private void Init(bool restore)
    {
        AIs = new ReactiveListNameSafe<AiObjectModel>();
        Buckets = new ReactiveListNameSafe<AiObjectModel>();
        Decisions = new ReactiveListNameSafe<AiObjectModel>();
        Considerations = new ReactiveListNameSafe<AiObjectModel>();
        AgentActions = new ReactiveListNameSafe<AiObjectModel>();
        ResponseCurves = new ReactiveListNameSafe<AiObjectModel>();

        collectionsByLabel = new Dictionary<string, ReactiveList<AiObjectModel>>();
        collectionsByLabel.Add(Consts.Label_UAIModel, AIs);
        collectionsByLabel.Add(Consts.Label_BucketModel, Buckets);
        collectionsByLabel.Add(Consts.Label_DecisionModel, Decisions);
        collectionsByLabel.Add(Consts.Label_ConsiderationModel, Considerations);
        collectionsByLabel.Add(Consts.Label_AgentActionModel, AgentActions);
        collectionsByLabel.Add(Consts.Label_ResponseCurve, ResponseCurves);

        foreach(var collection in collectionsByLabel)
        {
            collection.Value.OnValueChanged
                .Subscribe(col => onCollectionChanged.OnNext(collection.Value))
                .AddTo(subscriptions);
        }

        if (restore)
        {
            LoadAutoSave();
        }
    }

    internal void LoadAutoSave()
    {
        var perstistAPI = new PersistenceAPI(new JSONPersister());
        var state = perstistAPI.LoadObjectPath<UASTemplateServiceState>(Consts.File_UASTemplateServicel_AutoSave + Consts.FileExtension_JSON);
        if (state == null)
        {
            Debug.LogWarning("No playmode found");
            //LoadCollectionsFromFile();
        }
        else
        {
            try
            {
                Restore(state);
            } catch (Exception ex)
            {
                Debug.LogWarning("UASTemplateService Restore failed: " + ex);
            }
        }
    }

    internal void AutoSave()
    {
        var perstistAPI = new PersistenceAPI(new JSONPersister());
        perstistAPI.SaveObjectPath(this, Consts.File_UASTemplateServicel_AutoSave);
        Debug.Log("Autosave complete");
    }


    internal void Reset()
    {
        subscriptions.Clear();
        ClearCollections();
        Init(false);
    }

    internal static UASTemplateService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UASTemplateService();
            }
            return instance;
        }
    }

    internal Ai GetAiByName(string name, bool isPLayMode = false)
    {
        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Reset();
        //sw.Start();
        ////if (isPLayMode)
        ////{
        ////    LoadAutoSave();
        ////}
        //Debug.Log("Loaded Autosave in: " + sw.ElapsedMilliseconds + "ms");
        //sw.Reset();
        //sw.Start();

        var aiTemplate = AIs.Values
            .Cast<Ai>()
            .FirstOrDefault(ai => ai.Name == name && ai.IsPLayable);

        //Debug.Log("Sound templates in: " + sw.ElapsedMilliseconds + "ms");
        //sw.Reset();
        //sw.Start();

        if (aiTemplate == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogWarning("Ai: " + name + " not found, returning default Ai");
            }
            aiTemplate = AIs.Values.Cast<Ai>().First(ai => ai.IsPLayable);
            if (aiTemplate == null)
            {
                Debug.LogError("No ai found");
                throw new Exception("No default Ai found AiName: " + name + " is the ai playable?");
            }
        }
        //Debug.Log("Found default Ai in: " + sw.ElapsedMilliseconds + "ms");
        //sw.Reset();
        //sw.Start();

        var clone = aiTemplate.Clone() as Ai;

        //Debug.Log("Cloned Ai in: " + sw.ElapsedMilliseconds + "ms");
        //sw.Reset();
        //sw.Start();

        //sw.Stop();
        return clone;
    }

    internal ReactiveList<AiObjectModel> GetCollection(string label)
    {
        if (collectionsByLabel.ContainsKey(label))
        {
            return collectionsByLabel[label];
        } else
        {
            return null;
        }
    }

    internal ReactiveList<AiObjectModel> GetCollection(Type t)
    {
        if (t.IsAssignableFrom(typeof(Ai)))
        {
            return AIs;
        }
        if (t.IsAssignableFrom(typeof(Bucket)))
        {
            return Buckets;
        }
        if (t.IsAssignableFrom(typeof(Decision)))
        {
            return Decisions;
        }
        if (t.IsAssignableFrom(typeof(Consideration)))
        {
            return Considerations;
        }

        if (t.IsAssignableFrom(typeof(AgentAction)))
        {
            return AgentActions;
        }
        return null;
    }

    private ReactiveList<AiObjectModel> UpdateListWithFiles<T>(ReactiveList<AiObjectModel> collection)
    {
        var elementsFromFiles = AssetDatabaseService.GetInstancesOfType<T>();
        elementsFromFiles = elementsFromFiles
            .Where(e => collection.Values.FirstOrDefault(element => element.GetType().FullName == e.GetType().FullName) == null)
            .Where(e => !e.GetType().ToString().Contains("Mock"))
            .ToList();

        foreach(var element in elementsFromFiles)
        {
            collection.Add(element as AiObjectModel);
        }
        return collection;
    }

    //internal void Refresh()
    //{
    //    LoadCollectionsFromFile();
    //}



    private void OnEnable()
    {
        subscriptions.Clear();
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
        
        subscriptions.Clear();
    }

    private void OnDestroy()
    {
        subscriptions.Clear();
    }

    ~UASTemplateService()
    {
        subscriptions.Clear();
    }

    internal override RestoreState GetState()
    {
        return new UASTemplateServiceState(collectionsByLabel, AIs, Buckets, Decisions, Considerations, AgentActions, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }

    internal void Restore(UASTemplateServiceState state)
    {
        try
        {
            RestoreInternal(state,false);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("UASTemplateService Restore failed: " + ex);
        }

        //RestoreInternal(state);
    }

    internal void Add(AiObjectModel model)
    {
        var collection = GetCollection(model);
        collection.Add(model);
    }

    internal void Remove(AiObjectModel model)
    {
        var collection = GetCollection(model);
        collection.Remove(model);
    }

    private void ClearCollections() {
        subscriptions?.Clear();
        AIs?.Clear();
        Buckets?.Clear();
        Decisions?.Clear();
        Considerations?.Clear();
        AgentActions?.Clear();
        ResponseCurves?.Clear();
    }

    private ReactiveList<AiObjectModel> GetCollection(AiObjectModel model)
    {
        var type = model.GetType();

        if (TypeMatches(type, typeof(Consideration)))
        {
            return Considerations;
        }
        else if (TypeMatches(type,typeof(Decision)))
        {
            return Decisions;
        } else if (TypeMatches(type, typeof(AgentAction)))
        {
            return AgentActions;
        } else if (TypeMatches(type, typeof(Bucket)))
        {
            return Buckets;
        } else if (TypeMatches(type, typeof(Ai)))
        {
            return AIs;
        } else if (TypeMatches(type, typeof(ResponseCurve)))
        {
            return ResponseCurves;
        }
        return null;
    }

    private bool TypeMatches(Type a, Type b)
    {
        return a.IsAssignableFrom(b) || a.IsSubclassOf(b);
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug)
    {
        ClearCollections();
        var state = (UASTemplateServiceState)s;
        if (state == null)
        {
            return;
        }

        foreach(var aState in state.AIs)
        {
            var ai = Ai.Restore<Ai>(aState, restoreDebug);
            AIs.Add(ai);
        }

        foreach (var bState in state.Buckets)
        {
            var bucket = Bucket.Restore<Bucket>(bState, restoreDebug);
            Buckets.Add(bucket);
        }

        foreach (var d in state.Decisions)
        {
            var decision = Decision.Restore<Decision>(d, restoreDebug);
            Decisions.Add(decision);
        }

        foreach (var c in state.Considerations)
        {
            var consideration = Consideration.Restore<Consideration>(c, restoreDebug);
            Considerations.Add(consideration);
        }

        foreach (var a in state.AgentActions)
        {
            var action = AgentAction.Restore<AgentAction>(a, restoreDebug);
            AgentActions.Add(action);
        }

        foreach(var r in state.ResponseCurves)
        {
            var responseCurve = Restore<ResponseCurve>(r, restoreDebug);
            ResponseCurves.Add(responseCurve);
        }
    }

}

[Serializable]
public class UASTemplateServiceState : RestoreState
{
    public List<UAIModelState> AIs;
    public List<BucketState> Buckets;
    public List<DecisionState> Decisions;
    public List<ConsiderationState> Considerations;
    public List<AgentActionState> AgentActions;
    public List<ResponseCurveState> ResponseCurves;
    public string TestStrign = "";
    public string InternalTestString = "";
    public UASTemplateServiceState() : base()
    {
    }

    internal UASTemplateServiceState(
        Dictionary<string, ReactiveList<AiObjectModel>> collectionsByLabel, ReactiveList<AiObjectModel> aiS,
        ReactiveList<AiObjectModel> buckets, ReactiveList<AiObjectModel> decisions,
        ReactiveList<AiObjectModel> considerations, ReactiveList<AiObjectModel> agentActions, UASTemplateService model) : base(model)
    {
        AIs = new List<UAIModelState>();
        foreach (Ai ai in aiS.Values)
        {
            var a = ai.GetState() as UAIModelState;
            AIs.Add(a);
        }

        Buckets = new List<BucketState>();
        foreach (Bucket bucket in buckets.Values)
        {
            var b = bucket.GetState() as BucketState;
            Buckets.Add(b);
        }

        Decisions = new List<DecisionState>();
        foreach (Decision decision in decisions.Values)
        {
            var d = decision.GetState() as DecisionState;
            Decisions.Add(d);
        }

        Considerations = new List<ConsiderationState>();
        foreach (Consideration consideration in considerations.Values)
        {
            var c = consideration.GetState() as ConsiderationState;
            Considerations.Add(c);
        }

        AgentActions = new List<AgentActionState>();
        foreach (AgentAction action in agentActions.Values)
        {
            var a = action.GetState() as AgentActionState;
            AgentActions.Add(a);
        }

        ResponseCurves = new List<ResponseCurveState>();
        {
            foreach(ResponseCurve rc in model.ResponseCurves.Values)
            {
                var r = rc.GetState() as ResponseCurveState;
                ResponseCurves.Add(r);
            }
        }
    }

    internal static UASTemplateServiceState LoadFromFile()
    {
        Debug.Log("Change this");
        var p = new PersistenceAPI(new JSONPersister());
        var state = p.LoadObjectPanel<UASTemplateServiceState>();
        return state;
        //return (UASState)PersistenceAPI.LoadJson<UASState>(Consts.File_MainSavePath + Consts.FileName_UASModelJson);
    }
}
