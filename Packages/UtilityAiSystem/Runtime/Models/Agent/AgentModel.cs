using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public class AgentModel
{
    private string name;
    public string Name {
        get => name; 
        set
        {
            name = value;
            onNameChanged.OnNext(name);
        }
    }

    public IObservable<string> OnNameChanged => onNameChanged;
    private Subject<string> onNameChanged = new Subject<string> ();
}
