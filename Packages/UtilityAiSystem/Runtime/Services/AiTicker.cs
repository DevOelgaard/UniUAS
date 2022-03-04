using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using MoreLinq;
using UnityEngine;

internal class AiTicker: RestoreAble
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private static AiTicker instance;
    public static AiTicker Instance => instance ??= new AiTicker();
    private AgentManager agentManager => AgentManager.Instance;
    private int tickCount = 0;
    internal AiTickerSettingsModel Settings = new AiTickerSettingsModel();
    private PersistenceAPI persistenceAPI = new PersistenceAPI(new JSONPersister());

    internal AiTicker()
    {
        var loadedState = persistenceAPI.LoadObjectPath<AiTickerSettingsState>(Consts.Path_AiTickerSettings);
        Debug.Log("AiTicker: Loading");
        if (loadedState != null)
        {
            Settings = Restore<AiTickerSettingsModel>(loadedState);
        } else
        {
            Settings.TickerModes = new List<TickerMode>();
            Settings.TickerModes.Add(new TickerModeDesiredFrameRate());
            Settings.TickerModes.Add(new TickerModeTimeBudget());
            Settings.TickerModes.Add(new TickerModeUnrestricted());

            Settings.TickerMode = Settings.TickerModes.First(m => m.Name == AiTickerMode.Unrestricted);
        }
    }

    internal void Start()
    {
        Observable.TimerFrame(1)
            .Subscribe(_ => TickAis())
            .AddTo(disposables);
    }

    internal void TickAis()
    {
        tickCount++;
        var metaData = new TickMetaData();
        metaData.TickCount = tickCount;
        Settings.TickerMode.Tick(agentManager.Model.Agents.Values, metaData);
    }

    internal void SetTickerMode(AiTickerMode tickerMode)
    {
        var newMode = Settings.TickerModes.FirstOrDefault(m => m.Name == tickerMode);
        if (newMode == null) return;
        Settings.TickerMode = newMode;
    }

    protected override void RestoreInternal(RestoreState state)
    {
        var s = state as AiTickerState;
        Settings = Restore<AiTickerSettingsModel>(s.Settings);
    }

    internal override RestoreState GetState()
    {
        return new AiTickerState(Settings, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
    
    internal void Save()
    {
        persistenceAPI.SaveObjectPath(Settings, Consts.Path_AiTickerSettings);

    }

    ~AiTicker()
    {
        Save();
        disposables.Clear();
    }
}



[Serializable]
public class AiTickerState: RestoreState
{
    public AiTickerSettingsState Settings;

    public AiTickerState()
    {
    }

    internal AiTickerState(AiTickerSettingsModel settings, AiTicker o) : base(o)
    {
        Settings = settings.GetState() as AiTickerSettingsState;
    }
}
