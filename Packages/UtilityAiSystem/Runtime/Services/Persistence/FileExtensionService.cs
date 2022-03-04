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
        if (type.IsAssignableFrom(typeof(AgentAction)))
        {
            return Consts.FileExtension_AgentAction;
        }

        if (type.IsAssignableFrom(typeof(Consideration)))
        {
            return Consts.FileExtension_Consideration;
        }

        if (type.IsAssignableFrom(typeof(Decision)))
        {
            return Consts.FileExtension_Decision;
        }

        if (type.IsAssignableFrom(typeof(Bucket)))
        {
            return Consts.FileExtension_Bucket;
        }

        if (type.IsAssignableFrom(typeof(UAIModel)))
        {
            return Consts.FileExtension_UAI;
        }

        if (type.IsAssignableFrom(typeof(UASTemplateService)))
        {
            return Consts.FileExtension_UasTemplateCollection;
        }

        if (type.IsAssignableFrom(typeof(AiTickerSettingsModel)))
        {
            return Consts.FileExtension_TickerSettings;
        }
        return "";
    }

}
