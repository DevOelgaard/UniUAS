﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DecisionScoreEvaluator: IDecisionScoreEvaluator
{
    private string name = Consts.Name_DefaultDSE;
    private string description = Consts.Description_DefaultDSE; 
    public IUtilityContainerSelector DecisionSelector;
    public IUtilityContainerSelector BucketSelector;

    public DecisionScoreEvaluator(IUtilityContainerSelector decisionSelector = null, IUtilityContainerSelector bucketSelector = null)
    {
        if (decisionSelector == null)
        {
            decisionSelector = ScorerService.Instance.ContainerSelectors.Values.FirstOrDefault(cS => cS.GetName() == Consts.Default_DecisionSelector);
        }
        DecisionSelector = decisionSelector;
        if (bucketSelector == null)
        {
            bucketSelector = ScorerService.Instance.ContainerSelectors.Values.FirstOrDefault(cS => cS.GetName() == Consts.Default_BucketSelector);
        }
        BucketSelector = bucketSelector;
    }

    public string GetDescription()
    {
        return description;
    }

    public string GetName()
    {
        return name;
    }

    public List<AgentAction> NextActions(List<Decision> decisions, AiContext context)
    {
        if (decisions.Count == 0)
        {
            Debug.LogWarning("List of buckets must be >0");
            return null;
        } else 
        {
            var bestDecision = DecisionSelector.GetBestUtilityContainer(decisions, context);
            if (bestDecision.LastCalculatedUtility <= 0)
            {
                //Debug.LogWarning("No valid decision. Add a \"fall back\" decision (Ie. Idle), which always scores >0");
                return null;
            }

            return bestDecision.AgentActions.Values;
        }
    }

    public List<AgentAction> NextActions(List<Bucket> buckets, AiContext context)
    {
        if (buckets.Count == 0)
        {
            Debug.LogWarning("No valid decision in list of buckets. Add a \"fall back\" bucket, which always scores >0");
            return null;
        }
        //else if (buckets.Count == 1)
        //{
        //    return NextActions(buckets[0].Decisions.Values, context);
        //}
        else
        {
            var bestBucket = BucketSelector.GetBestUtilityContainer(buckets, context);
            var bestAction = NextActions(bestBucket.Decisions.Values, context);
            if (bestAction == null)
            {
                // No valid decision in most valid bucket
                // Recursive selection to find valid decision in next most valid bucket
                return NextActions(buckets.Where(bucket => bucket != bestBucket).ToList(), context);
            }
            else
            {
                return bestAction;
            }
        }
    }
}
