using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class AiLogComponent : AiObjectLogComponent
{
    private LogComponentPool<BucketLogComponent> bucketPool;
    public AiLogComponent(): base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        bucketPool = new LogComponentPool<BucketLogComponent>(root);
    }

    protected override void DisplayInternal(AiObjectLog aiObjectDebug)
    {
        var a = (AiLog)aiObjectDebug;
        base.Display(aiObjectDebug);

        var logModels = new List<ILogModel>();
        foreach (var b in a.Buckets)
        {
            logModels.Add(b);
        }
        bucketPool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        bucketPool.Hide();
    }
}