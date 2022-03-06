using System;

internal class MainWindowService
{
    internal static MainWindowComponent GetComponent(AiObjectModel model)
    {
        var type = model.GetType();
        if (type == typeof(UAIModel))
        {
            return new UAIComponent(model as UAIModel);
        }
        else if (type == typeof(Bucket))
        {
            return new BucketComponent(model as Bucket);
        }
        else if (type == typeof(Decision) || type.IsSubclassOf(typeof(Decision)))
        {
            return new DecisionComponent(model as Decision);
        }
        else if (type.IsSubclassOf(typeof(Consideration)))
        {
            return new ConsiderationComponent(model as Consideration);
        }
        else if (type.IsSubclassOf(typeof(AgentAction)))
        {
            return new AgentActionComponent(model as AgentAction);
        }
        throw new NotImplementedException();
    }

    internal static Type GetTypeFromString(string label)
    {
        switch (label)
        {
            case Consts.Label_UAIModel:
                return typeof(UAIModel);
            case Consts.Label_BucketModel:
                return typeof(Bucket);
            case Consts.Label_DecisionModel:
                return typeof(Decision);
            case Consts.Label_ConsiderationModel:
                return typeof(Consideration);
            case Consts.Label_AgentActionModel:
                return typeof(AgentAction);
            default:
                break;
        }
        return null;
    }
}