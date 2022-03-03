using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class AiTickerSettingsModel
{
    internal int ThreadCount = 10;
    internal int MaxTicksPrFrame = 100;
    internal int MaxTicksPrMillisecond = 100;
    internal string Description;
    private TickerMode tickerMode;
    internal TickerMode TickerMode
    {
        get => tickerMode;
        set
        {
            tickerMode = value;
            onTickerModeChanged.OnNext(tickerMode);
        }
    }
    internal IObservable<TickerMode> OnTickerModeChanged => onTickerModeChanged;
    private Subject<TickerMode> onTickerModeChanged = new Subject<TickerMode>();


    internal List<TickerMode> TickerModes;


}
