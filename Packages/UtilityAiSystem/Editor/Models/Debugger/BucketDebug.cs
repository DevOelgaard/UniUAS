using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BucketDebug: AiObjectDebug
{
    internal List<ConsiderationDebug> Considerations;
    internal List<DecisionDebug> Decisions;
    internal float Score;
    internal float Weight;

    internal static BucketDebug GetDebug(Bucket bucket)
    {
        var bucketDebug = new BucketDebug();
        bucketDebug.Name = bucket.Name;
        bucketDebug.Description = bucket.Description;
        bucketDebug.Type = bucket.GetType().ToString();
        bucketDebug.Score = bucket.ScoreModels.First().Value;
        bucketDebug.Weight = Convert.ToSingle(bucket.Weight.Value);

        bucketDebug.Considerations = new List<ConsiderationDebug>();
        foreach (var consideration in bucket.Considerations.Values)
        {
            bucketDebug.Considerations.Add(ConsiderationDebug.GetDebug(consideration));
        }

        return bucketDebug;
    }
}