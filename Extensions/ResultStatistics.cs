using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

public struct ResultDescriptor
{
    public int Epoch;
    public float HitResponse;
    public float FalseAlarm;
}

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class ResultStatistics
{
    public IObservable<ResultDescriptor> Process(IObservable<IList<ResultId>> source)
    {
        return source.Select(result =>
        {
            var stats = new ResultDescriptor();
            foreach (var item in result)
            {
                switch (item)
                {
                    case ResultId.HitResponse: stats.HitResponse++; break;
                    case ResultId.FalseAlarm: stats.FalseAlarm++; break;
                }
            }
            stats.HitResponse/=result.Count;
            stats.FalseAlarm/=result.Count;
            return stats;
        });
    }
}
