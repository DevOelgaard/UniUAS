using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AiDebug: AiObjectDebug
{
    internal List<BucketDebug> Buckets;
    internal List<DecisionDebug> Decisions;

    internal static AiDebug GetDebug(Ai ai)
    {
        var result = new AiDebug();
        result.Name = ai.Name;
        result.Description = ai.Description;
        result.Type = ai.GetType().ToString();
        result.Buckets = new List<BucketDebug>();
        foreach (var bucket in ai.Buckets.Values)
        {
            result.Buckets.Add(BucketDebug.GetDebug(bucket));
        }
        return result;
    }
}