using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UCSHighestScore : IUtilityContainerSelector
{
    private string name = Consts.Name_UCSHighestScore;
    private string description = Consts.Description_UCSHighestScore;

    public string GetDescription()
    {
        return description;
    }

    public string GetName()
    {
        return name;
    }

    public Bucket GetBestUtilityContainer(List<Bucket> containers, AiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach(Bucket container in containers)
        {
            var weight = Convert.ToSingle(container.Weight.Value);

            // Only evaluate if the bucket has a chance of winning
            if (bestContainer != null && weight < bestContainer.LastCalculatedUtility)
            {
                continue;
            }
            context.CurrentEvaluatedBucket = container;
            bestContainer = CheckBestContainer(container, context, bestContainer);
        }
        context.LastSelectedBucket = bestContainer as Bucket;
        return bestContainer as Bucket;
    }

    public Decision GetBestUtilityContainer(List<Decision> containers, AiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach (var container in containers)
        {
            context.CurrentEvalutedDecision = container;
            bestContainer = CheckBestContainer(container, context, bestContainer);
        }
        context.LastSelectedDecision = bestContainer as Decision;
        return bestContainer as Decision;
    }

    private UtilityContainer CheckBestContainer(UtilityContainer container, AiContext context, UtilityContainer bestContainer = null)
    {
        var utility = container.GetUtility(context);
        if (utility <= 0)
        {
            return bestContainer;
        }

        if (bestContainer == null)
        {
            return container;
        } else
        {
            if(utility > bestContainer.LastCalculatedUtility)
            {
                return container;
            }
        }
        return bestContainer;
    }
}
