using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AiLog: AiObjectLog
{
    internal List<BucketLog> Buckets;

    internal static AiLog GetDebug(Ai ai)
    {
        var result = new AiLog();
        result.Name = ai.Name;
        result.Description = ai.Description;
        result.Type = ai.GetType().ToString();
        result.Buckets = new List<BucketLog>();
        foreach (var bucket in ai.Buckets.Values)
        {
            result.Buckets.Add(BucketLog.GetDebug(bucket));
        }
        return result;
    }
}