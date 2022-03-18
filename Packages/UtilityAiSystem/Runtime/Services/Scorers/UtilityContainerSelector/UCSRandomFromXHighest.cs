using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class UCSRandomFromXHighest : UtilityContainerSelector
{
    private int NumberOfItemsToEvaluate
    {
        get
        {
            var val = Convert.ToInt32(Parameters[0].Value);
            if (val > 0)
            {
                return val;
            } else
            {
                return int.MaxValue;
            }       
        }
    }
    private bool PercentageChance => (bool)Parameters[1].Value;

    private float MaxDeviationFromHighest => Convert.ToSingle(Parameters[2].Value);
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Number Of Items", 1),
            new Parameter("Percentage chance", true),
            new Parameter("Max deviation from highest", 1f)
        };
    }
    public override Bucket GetBestUtilityContainer(List<Bucket> buckets, AiContext context)
    {
        var sortedList = new SortedList<float, UtilityContainer>();
        foreach(Bucket bucket in buckets)
        {
            var lowestValidValue = GetLowestValidValue(sortedList);
            if (Convert.ToSingle(bucket.Weight.Value) < lowestValidValue)
            {
                continue;
            } else
            {
                context.CurrentEvaluatedBucket = bucket;
                bucket.GetUtility(context);
                UpdateList(sortedList, bucket);
            }
        }
        return (Bucket)GetRandomContainer(sortedList);

    }

    public override Decision GetBestUtilityContainer(List<Decision> decisions, AiContext context)
    {
        var sortedList = new SortedList<float, UtilityContainer>();
        foreach (Decision decision in decisions)
        {
            context.CurrentEvalutedDecision = decision;
            decision.GetUtility(context);
            UpdateList(sortedList, decision);
        }
        return (Decision)GetRandomContainer(sortedList);
    }

    public override string GetDescription()
    {
        return Consts.UCS_RandomXHighest_Description;
    }

    public override string GetName()
    {
        return Consts.UCS_RandomXHighest_Name;
    }

    private void UpdateList(SortedList<float, UtilityContainer> list, UtilityContainer container)
    {
        if (container.LastCalculatedUtility <= 0 )
        {
            return;
        }

        // Return if score is to far from highest score.
        var highestValid = list.FirstOrDefault().Value;
        if (highestValid != null)
        {
            var minimumAllowedScore = highestValid.LastCalculatedUtility - MaxDeviationFromHighest;
            if (container.LastCalculatedUtility < minimumAllowedScore)
            {
                return;
            }
        }

        var evaluateIndex = NumberOfItemsToEvaluate - 1;

        if (list.Count < NumberOfItemsToEvaluate)
        {
            list.Add(container.LastCalculatedUtility, container);
        } 
        else if (container.LastCalculatedUtility < list.Values[evaluateIndex].LastCalculatedUtility)
        {
            return;
        } 
        else if (container.LastCalculatedUtility < list.Values[evaluateIndex].LastCalculatedUtility)
        {
            var rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) // Swapping two equally scored containers at random
            {
                list[evaluateIndex] = container;
            }
        } 
        else
        {
            list.Add(container.LastCalculatedUtility, container);
        }
    }

    private UtilityContainer GetRandomContainer(SortedList<float, UtilityContainer> list)
    {
        if (list.Count == 0)
        {
            throw new Exception("No items to chose from, list.Count must be > 0");
        }
        if (PercentageChance)
        {
            var sum = list
                .Values
                .Take(NumberOfItemsToEvaluate)
                .Where(uc => uc != null)
                .Sum(uc => uc.LastCalculatedUtility);
            

            var resultNumber = UnityEngine.Random.Range(0, sum);
            for(var i = 0; i < NumberOfItemsToEvaluate; i++)
            {
                resultNumber -= list.Values[i].LastCalculatedUtility;
                if(resultNumber <= 0)
                {
                    return list.Values[i];
                }
            }
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, NumberOfItemsToEvaluate);
            return list.Values[rand];
        }

        throw new Exception("Something should have been chosen at this point");
    }
    private float GetLowestValidValue(SortedList<float, UtilityContainer> list)
    {
        if (list.Count() == 0)
        {
            return default;
        }
        else if (list.Count() < NumberOfItemsToEvaluate)
        {
            return list.Last().Key;
        }
        else
        {
            return list.Values[NumberOfItemsToEvaluate - 1].LastCalculatedUtility;
        }
    }
}