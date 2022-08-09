using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

public struct TwoAfcDescriptor
{
    public int Epoch;
    public int LeftHits;
    public int RightHits;
    public int CorrectActions;
    public int IncorrectActions;
    public int EarlyResponses;
    public int TotalHits;
    public int TotalLeftTrials;
    public int TotalRightTrials;

}

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class TwoAfcStatistics
{
    void UpdateSlidingStatistics(ref TwoAfcDescriptor stats, ResponseId response)
    {
        stats.Epoch++;
        switch (response)
        {
            case ResponseId.LeftHit: stats.LeftHits++; break;
            case ResponseId.RightHit: stats.RightHits++; break;
        }
    }
    void UpdateTotalStatistics(ref TwoAfcDescriptor stats, ResponseId response)
    {
        stats.Epoch++;
        switch (response)
        {
            case ResponseId.LeftHit: stats.TotalLeftTrials++; stats.TotalHits++; break;
            case ResponseId.RightHit: stats.TotalRightTrials++; stats.TotalHits++; break;
        }
    }
    public IObservable<TwoAfcDescriptor> Process(IObservable<ResponseId> source)
    {
        return source.Scan(new TwoAfcDescriptor(), (stats, response) =>
        {
            UpdateSlidingStatistics(ref stats, response);
            return stats;
        });
        
    }

    public IObservable<TwoAfcDescriptor> Process(IObservable<IList<ResponseId>> source)
    {
        return source.Scan(new TwoAfcDescriptor(), (stats, responses) =>
        {
            UpdateTotalStatistics (ref stats, responses[responses.Count - 1]);
            foreach(var response in responses)
            {
                 UpdateSlidingStatistics(ref stats, response);
            }
            return stats;
        });
        
    }
}
