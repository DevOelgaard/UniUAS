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
    public UtilityContainerSelector DecisionSelector;
    public UtilityContainerSelector BucketSelector;

    public DecisionScoreEvaluator(UtilityContainerSelector decisionSelector, UtilityContainerSelector bucketSelector)
    {
        DecisionSelector = decisionSelector;
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
            if (bestDecision == null || bestDecision.LastCalculatedUtility <= 0)
            {
                Debug.LogWarning("No valid decision. Add a \"fall back\" decision (Ie. Idle), which always scores >0");

                //throw new Exception("No valid decision. Add a \"fall back\" decision (Ie. Idle), which always scores >0");
            }
            context.LastSelectedDecision = bestDecision;
            bestDecision.MetaData.LastTickSelected = context.TickMetaData.TickCount;
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
            if (bestBucket == null)
            {
                Debug.LogWarning("No valid bucket. Add a \"fall back\" decision (Ie. Idle), which always scores >0");
                //throw new Exception("No valid decision. Add a \"fall back\" decision (Ie. Idle), which always scores >0");
            }
            var bestAction = NextActions(bestBucket.Decisions.Values, context);
            if (bestAction == null)
            {
                // No valid decision in most valid bucket
                // Recursive selection to find valid decision in next most valid bucket
                return NextActions(buckets.Where(bucket => bucket != bestBucket).ToList(), context);
            }
            else
            {
                context.LastSelectedBucket = bestBucket;
                bestBucket.MetaData.LastTickSelected = context.TickMetaData.TickCount;
                return bestAction;
            }
        }
    }
}
