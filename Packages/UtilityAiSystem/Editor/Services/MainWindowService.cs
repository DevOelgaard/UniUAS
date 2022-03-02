using System;

internal class MainWindowService
{
    internal static MainWindowComponent GetComponent(MainWindowModel model)
    {
        if (model.GetType() == typeof(UAIModel))
        {
            return new UAIComponent(model as UAIModel);
        }
        else if (model.GetType() == typeof(Bucket))
        {
            return new BucketComponent(model as Bucket);
        }
        else if (model.GetType() == typeof(Decision))
        {
            return new DecisionComponent(model as Decision);
        }
        else if (model.GetType().IsSubclassOf(typeof(Consideration)))
        {
            return new ConsiderationComponent(model as Consideration);
        }
        else if (model.GetType().IsSubclassOf(typeof(AgentAction)))
        {
            return new AgentActionComponent(model as AgentAction);
        }
        throw new NotImplementedException();
    }

    internal static Type GetTypeFromString(string label)
    {
        switch (label)
        {
            case Statics.Label_UAIModel:
                return typeof(UAIModel);
            case Statics.Label_BucketModel:
                return typeof(Bucket);
            case Statics.Label_DecisionModel:
                return typeof(Decision);
            case Statics.Label_ConsiderationModel:
                return typeof(Consideration);
            case Statics.Label_AgentActionModel:
                return typeof(AgentAction);
            default:
                break;
        }
        return null;
    }

    //public static string MainWindowModelToString(MainWindowModel model)
    //{
    //    return model.Name;
    //}
}