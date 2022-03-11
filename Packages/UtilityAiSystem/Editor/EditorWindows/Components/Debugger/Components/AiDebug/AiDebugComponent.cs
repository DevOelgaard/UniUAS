using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class AiDebugComponent : AiObjectDebugComponent
{
    public AiDebugComponent(AiDebug a) : base(a)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        foreach(var b in a.Buckets)
        {
            Add(new BucketDebugComponent(b));
        }
    }
}