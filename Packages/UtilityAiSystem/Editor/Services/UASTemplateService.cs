using UnityEngine;
using UniRx;
using UniRxExtension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UASTemplateService: RestoreAble
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private Dictionary<string, ReactiveList<MainWindowModel>> collectionsByLabel = new Dictionary<string, ReactiveList<MainWindowModel>>();

    public ReactiveList<MainWindowModel> AIs;
    public ReactiveList<MainWindowModel> Buckets;
    public ReactiveList<MainWindowModel> Decisions;
    public ReactiveList<MainWindowModel> Considerations;
    public ReactiveList<MainWindowModel> AgentActions;

    private static UASTemplateService instance;

    public UASTemplateService()
    {
        Init();
    }

    private UASTemplateService(bool restore)
    {
        Init();
        Debug.Log("Constructiong");

        if (restore)
        {
            LoadFromFile();
        } 
    }

    private void Init()
    {
        AIs = new ReactiveList<MainWindowModel>();
        Buckets = new ReactiveList<MainWindowModel>();
        Decisions = new ReactiveList<MainWindowModel>();
        Considerations = new ReactiveList<MainWindowModel>();
        AgentActions = new ReactiveList<MainWindowModel>();

        collectionsByLabel = new Dictionary<string, ReactiveList<MainWindowModel>>();
        collectionsByLabel.Add(Statics.Label_UAIModel, AIs);
        collectionsByLabel.Add(Statics.Label_BucketModel, Buckets);
        collectionsByLabel.Add(Statics.Label_DecisionModel, Decisions);
        collectionsByLabel.Add(Statics.Label_ConsiderationModel, Considerations);
        collectionsByLabel.Add(Statics.Label_AgentActionModel, AgentActions);

        LoadCollectionsFromFile();
    }


    // TODO Remove, only used for testing
    internal void Reset()
    {
        subscriptions.Clear();
        ClearCollections();
        Init();
        LoadCollectionsFromFile();
    }

    private void LoadCollectionsFromFile()
    {
        Considerations = UpdateListWithFiles<Consideration>(Considerations);
        AgentActions = UpdateListWithFiles<AgentAction>(AgentActions);
    }

    public static UASTemplateService Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("Creating new UAS-Model");
                instance = new UASTemplateService(true);
            }
            return instance;
        }
    }

    public ReactiveList<MainWindowModel> GetCollection(string label)
    {
        if (collectionsByLabel.ContainsKey(label))
        {
            return collectionsByLabel[label];
        } else
        {
            return null;
        }
    }

    private ReactiveList<MainWindowModel> UpdateListWithFiles<T>(ReactiveList<MainWindowModel> collection)
    {
        var elementsFromFiles = AssetDatabaseService.GetTypeFromFile<T>();
        elementsFromFiles = elementsFromFiles
            .Where(e => collection.Values.FirstOrDefault(element => element.GetType() == e.GetType()) == null)
            .Where(e => !e.GetType().ToString().Contains("Mock"))
            .ToList();

        foreach(var element in elementsFromFiles)
        {
            collection.Add(element as MainWindowModel);
        }
        return collection;
    }

    internal void LoadFromFile()
    {
        var state = UASState.LoadFromFile();
        if(state != null)
        {
            RestoreInternal(state);
        }
    }

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

    public bool SaveToFile()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Reset();
        sw.Start();
        var state = new UASState(collectionsByLabel, AIs, Buckets, Decisions, Considerations, AgentActions, this);
        PersistenceAPI.PersistJson<UASState>(state, Statics.File_MainSavePath, Statics.FileName_UASModelJson);
        sw.Stop();
        var elapsed = sw.Elapsed;
        Debug.Log("Save time: " + elapsed.Milliseconds +"ms");
        return true;
    }

    internal void Add(MainWindowModel model)
    {
        var collection = GetCollection(model);
        collection.Add(model);
    }

    internal void Remove(MainWindowModel model)
    {
        var collection = GetCollection(model);
        collection.Remove(model);
    }

    private void ClearCollections() {
        subscriptions.Clear();
        AIs.Clear();
        Buckets.Clear();
        Decisions.Clear();
        Considerations.Clear();
        AgentActions.Clear();
    }

    private ReactiveList<MainWindowModel> GetCollection(MainWindowModel model)
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
        } else if (TypeMatches(type, typeof(UAIModel)))
        {
            return AIs;
        }
        return null;
    }

    private bool TypeMatches(Type a, Type b)
    {
        return a.IsAssignableFrom(b) || a.IsSubclassOf(b);
    }

    protected override void RestoreInternal(RestoreState s)
    {
        ClearCollections();
        var state = (UASState)s;
        if (state == null)
        {
            return;
        }

        foreach(var aState in state.AIs)
        {
            var ai = UAIModel.Restore<UAIModel>(aState);
            AIs.Add(ai);
        }

        foreach (var bState in state.Buckets)
        {
            var bucket = Bucket.Restore<Bucket>(bState);
            Buckets.Add(bucket);
        }

        foreach (var d in state.Decisions)
        {
            var decision = Decision.Restore<Decision>(d);
            Decisions.Add(decision);
        }

        foreach (var c in state.Considerations)
        {
            var consideration = Consideration.Restore<Consideration>(c);
            Considerations.Add(consideration);
        }

        foreach (var a in state.AgentActions)
        {
            var action = AgentAction.Restore<AgentAction>(a);
            AgentActions.Add(action);
        }
    }

    [Serializable]
    public class UASState: RestoreState
    {
        public List<UAIModelState> AIs;
        public List<BucketState> Buckets;
        public List<DecisionState> Decisions;
        public List<ConsiderationState> Considerations;
        public List<AgentActionState> AgentActions;
        public string TestStrign = "";
        public string InternalTestString = "";
        public UASState(): base()
        {
        }
        
        public UASState(
            Dictionary<string, ReactiveList<MainWindowModel>> collectionsByLabel, ReactiveList<MainWindowModel> aiS, 
            ReactiveList<MainWindowModel> buckets, ReactiveList<MainWindowModel> decisions, 
            ReactiveList<MainWindowModel> considerations, ReactiveList<MainWindowModel> agentActions, UASTemplateService model): base(model)
        {
            AIs = new List<UAIModelState>();
            foreach(UAIModel ai in aiS.Values)
            {
                var a = ai.GetState();
                AIs.Add(a);
            }

            Buckets = new List<BucketState>();
            foreach(Bucket bucket in buckets.Values)
            {
                var b = bucket.GetState();
                Buckets.Add(b);
            }

            Decisions = new List<DecisionState>();
            foreach(Decision decision in decisions.Values)
            {
                var d = decision.GetState();
                Decisions.Add(d);
            }
            
            Considerations = new List<ConsiderationState>();
            foreach(Consideration consideration  in considerations.Values)
            {
                var c = consideration.GetState();
                Considerations.Add(c);
            }

            AgentActions = new List<AgentActionState>();
            foreach(AgentAction action in agentActions.Values)
            {
                var a = action.GetState();
                AgentActions.Add(a);
            }
        }

        internal static UASState LoadFromFile()
        {
            return (UASState)PersistenceAPI.LoadJson<UASState>(Statics.File_MainSavePath + Statics.FileName_UASModelJson);
        }
    }
}
