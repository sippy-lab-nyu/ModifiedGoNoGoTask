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
}

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class ResponseStatistics
{
    public IObservable<ResponseDescriptor> Process(IObservable<ResponseId> source)
    {
        return source.Scan(new ResponseDescriptor(), (stats, response) =>
        {
            switch (response)
            {
                case ResponseId.Hit: stats.Hits++; break;
                case ResponseId.Miss: stats.Misses++; break;
                case ResponseId.FalseAlarm: stats.FalseAlarms++; break;
                case ResponseId.CorrectRejection: stats.CorrectRejections++; break;
            }
            return stats;
        });
    }
}
