using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BucketLog: AiObjectLog
{
    internal List<ConsiderationLog> Considerations;
    internal List<DecisionLog> Decisions;
    internal float Score;
    internal float Weight;

    internal static BucketLog GetDebug(Bucket bucket)
    {
        var bucketDebug = new BucketLog();
        bucketDebug.Name = bucket.Name;
        bucketDebug.Description = bucket.Description;
        bucketDebug.Type = bucket.GetType().ToString();
        bucketDebug.Score = bucket.ScoreModels.First().Value;
        bucketDebug.Weight = Convert.ToSingle(bucket.Weight.Value);

        bucketDebug.Considerations = new List<ConsiderationLog>();
        foreach (var consideration in bucket.Considerations.Values)
        {
            bucketDebug.Considerations.Add(ConsiderationLog.GetDebug(consideration));
        }

        bucketDebug.Decisions = new List<DecisionLog>();
        foreach(var decision in bucket.Decisions.Values)
        {
            bucketDebug.Decisions.Add(DecisionLog.GetDebug(decision));
        }

        return bucketDebug;
    }
}