using System;

internal class MainWindowService
{
    internal static AiObjectComponent  GetComponent(AiObjectModel model)
    {


        var type = model.GetType();
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
        } else if (type == typeof(ResponseCurve) || type.IsSubclassOf(typeof(ResponseCurve)))
        {
            return new ResponseCurveMainWindowComponent();
        }
        throw new NotImplementedException();
    }

    internal static Type GetTypeFromString(string label)
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
}