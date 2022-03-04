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
        return GetExtension(type, o);
    }

    internal static string GetExtension(Type type, object o)
    {
        var result = "";
        if (type.IsAssignableFrom(typeof(AgentAction)))
        {
            result = Consts.FileExtension_AgentAction;
        }

        if (type.IsAssignableFrom(typeof(Consideration)))
        {
            result = Consts.FileExtension_Consideration;
        }

        if (type.IsAssignableFrom(typeof(Decision)))
        {
            result = Consts.FileExtension_Decision;
        }

        if (type.IsAssignableFrom(typeof(Bucket)))
        {
            result = Consts.FileExtension_Bucket;
        }

        if (type.IsAssignableFrom(typeof(UAIModel)))
        {
            result = Consts.FileExtension_UAI;
        }

        if (type.IsAssignableFrom(typeof(UASTemplateService)))
        {
            result = Consts.FileExtension_UasTemplateCollection;
        }

        if (type.IsAssignableFrom(typeof(AiTickerSettingsModel)))
        {
            result = Consts.FileExtension_TickerSettings;
        }

        if (type.IsAssignableFrom(typeof(RestoreAbleCollection)))
        {
            var cast = o as RestoreAbleCollection;
            var t = cast.Type;
            result = GetExtension(t, o) + Consts.FileExtension_RestoreAbleCollection;
        }
        return result;
    }

}
