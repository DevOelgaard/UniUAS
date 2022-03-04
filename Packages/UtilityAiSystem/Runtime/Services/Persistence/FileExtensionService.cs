using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FileExtensionService
{
    internal static string GetExtension(object o)
    {
        var type = o.GetType();
        return GetExtension(type);
    }

    internal static string GetExtension(Type type)
    {
        if (type.IsAssignableFrom(typeof(AgentActionState)))
        {
            return Consts.FileExtension_AgentAction;
        }

        if (type.IsAssignableFrom(typeof(ConsiderationState)))
        {
            return Consts.FileExtension_Consideration;
        }

        if (type.IsAssignableFrom(typeof(DecisionState)))
        {
            return Consts.FileExtension_Decision;
        }

        if (type.IsAssignableFrom(typeof(BucketState)))
        {
            return Consts.FileExtension_Bucket;
        }

        if (type.IsAssignableFrom(typeof(UAIModelState)))
        {
            return Consts.FileExtension_UAI;
        }

        if (type.IsAssignableFrom(typeof(UASTemplateServiceState)))
        {
            return Consts.FileExtension_UasTemplate;
        }

        if (type.IsAssignableFrom(typeof(AiTickerSettingsState)))
        {
            return Consts.FileExtension_TickerSettings;
        }
        return "";
    }

}
