using System;
using UniRx;
using System.Collections.Generic;

public abstract class MainWindowModel: RestoreAble
{
    public IObservable<string> OnNameChanged => onNameChanged;
    private Subject<string> onNameChanged = new Subject<string>();

    public IObservable<string> OnDescriptionChanged => onDescriptionChanged;
    private Subject<string> onDescriptionChanged = new Subject<string>();

    public List<ScoreModel> ScoreModels = new List<ScoreModel>();

    protected MainWindowModel()
    {
        
    }

    internal abstract MainWindowModel Clone();

    public virtual string GetNameFormat(string name)
    {
        return name;
    }

    protected class MainWindowModelState
    {
        public string Name;
        public string Description;

        public MainWindowModelState(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }


    private string name;
    public string Name
    {
        get
        {
            if (String.IsNullOrEmpty(name))
            {
                Name = "Default";
            }
            return name;
        }
        set
        {
            name = GetNameFormat(value);
            onNameChanged.OnNext(Name);
        }
    }


    private string description;
    public string Description
    {
        get => description;
        set
        {
            if (value == description)
                return;
            description = value;
            onDescriptionChanged.OnNext(description);
        }
    }
}
