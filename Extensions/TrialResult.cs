using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class TrialResult : INamedElement
{
    public ResultId Result { get; set; }

    string INamedElement.Name
    {
        get { return Result.ToString(); }
    }

    public IObservable<ResultId> Process<TSource>(IObservable<TSource> source)
    {
        return source.Select(value => Result);
    }
}
