using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class MainWindowService
{
    private static MainWindowService instance;
    private Dictionary<Type, Queue<AiObjectComponent>> componentsByType = new Dictionary<Type, Queue<AiObjectComponent>>();
    private List<Type> typeList = new List<Type>()
    {
        typeof(Ai),
        typeof(Bucket),
        typeof(Decision),
        typeof(Consideration),
        typeof(AgentAction),
        typeof(ResponseCurve)
    };
    
    public MainWindowService()
    {
        foreach(var type in typeList)
        {
            InitComponentsTask(type, 10);
        }
    }

    private void InitComponentsTask(Type t, int amount)
    {
        var task = new Task(_ => InitComponents(t, amount), "");
        task.Start();
    }

    private void InitComponents(Type t, int amount)
    {
        if (!componentsByType.ContainsKey(t))
        {
            componentsByType.Add(t, new Queue<AiObjectComponent>());
        }
        for(var i = 0; i < amount; i++)
        {
            var component = GetComponent(t);
            componentsByType[t].Enqueue(component);
        }
    }

    internal AiObjectComponent RentComponent(AiObjectModel model)
    {
        var type = model.GetType();
        return RentComponent(type);
    }

    internal AiObjectComponent RentComponent(Type type)
    {
        if (!componentsByType.ContainsKey(type))
        {
            InitComponentsTask(type, 10);
            return GetComponent(type);
        }

        if (componentsByType[type].Count <= 0)
        {
            InitComponentsTask(type, 10);

            return GetComponent(type);
        }
        return componentsByType[type].Dequeue();
    }

    internal void ReturnComponent(AiObjectComponent component)
    {
        var type = component.Model.GetType();
        if (!componentsByType.ContainsKey(type))
        {
            componentsByType.Add(type, new Queue<AiObjectComponent>());
        }
        componentsByType[type].Enqueue(component);
    }


    private AiObjectComponent GetComponent(AiObjectModel model)
    {
        var type = model.GetType();
        return GetComponent(type);
    }
    private AiObjectComponent GetComponent(Type type)
    {
        if (type == typeof(Ai) || type.IsSubclassOf(typeof(Ai)))
        {
            return new AiComponent();
        }
        else if (type == typeof(Bucket) || type.IsSubclassOf(typeof(Bucket)))
        {
            return new BucketComponent();
        }
        else if (type == typeof(Decision) || type.IsSubclassOf(typeof(Decision)))
        {
            return new DecisionComponent();
        }
        else if (type.IsSubclassOf(typeof(Consideration)))
        {
            return new ConsiderationComponent();
        }
        else if (type.IsSubclassOf(typeof(AgentAction)))
        {
            return new AgentActionComponent();
        }
        else if (type == typeof(ResponseCurve) || type.IsSubclassOf(typeof(ResponseCurve)))
        {
            return new ResponseCurveMainWindowComponent();
        }
        throw new NotImplementedException();
    }


    internal Type GetTypeFromString(string label)
    {
        switch (label)
        {
            case Consts.Label_UAIModel:
                return typeof(Ai);
            case Consts.Label_BucketModel:
                return typeof(Bucket);
            case Consts.Label_DecisionModel:
                return typeof(Decision);
            case Consts.Label_ConsiderationModel:
                return typeof(Consideration);
            case Consts.Label_AgentActionModel:
                return typeof(AgentAction);
            case Consts.Label_ResponseCurve:
                return typeof(ResponseCurve);
            default:
                break;
        }
        return null;
    }

    internal static MainWindowService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MainWindowService();
            }
            return instance;
        }
    }
}