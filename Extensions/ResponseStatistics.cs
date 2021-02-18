using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

public struct ResponseDescriptor
{
    public int Epoch;
    public int Hits;
    public int Misses;
    public int FalseAlarms;
    public int CorrectRejections;
    public int PullPenalty;
    public int EarlyResponse;
}

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class ResponseStatistics
{
    void UpdateStatistics(ref ResponseDescriptor stats, ResponseId response)
    {
        stats.Epoch++;
        switch (response)
        {
            case ResponseId.Hit: stats.Hits++; break;
            case ResponseId.Miss: stats.Misses++; break;
            case ResponseId.FalseAlarm: stats.FalseAlarms++; break;
            case ResponseId.CorrectRejection: stats.CorrectRejections++; break;
        }
    }

    public IObservable<ResponseDescriptor> Process(IObservable<ResponseId> source)
    {
        return source.Scan(new ResponseDescriptor(), (stats, response) =>
        {
            UpdateStatistics(ref stats, response);
            return stats;
        });
    }

    public IObservable<ResponseDescriptor> Process(IObservable<IList<ResponseId>> source)
    {
        return source.Select(responses =>
        {
            var stats = new ResponseDescriptor();
            foreach(var response in responses)
            {
                UpdateStatistics(ref stats, response);
            }
            return stats;
        });
    }
}
