using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UCSHighestScore : IUtilityContainerSelector
{
    private string name = Statics.Name_UCSHighestScore;
    private string description = Statics.Description_UCSHighestScore;

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
        foreach(var container in containers)
        {
            bestContainer = CheckBestContainer(container, context, bestContainer);
        }
        return bestContainer as Bucket;
    }

    public Decision GetBestUtilityContainer(List<Decision> containers, AiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach (var container in containers)
        {
            bestContainer = CheckBestContainer(container, context, bestContainer);
        }
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
