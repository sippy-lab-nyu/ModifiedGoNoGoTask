using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class SampleState
{
    public IObservable<StateInfo> Process(IObservable<StateInfo> source, IObservable<double> sampler)
    {
        return Observable.Create<StateInfo>(observer =>
        {
            var currentState = default(StateInfo);
            var synchronized = Observer.Synchronize(observer);
            var stateObserver = Observer.Create<StateInfo>(
                state =>
                {
                    synchronized.OnNext(state);
                    currentState = state;
                },
                synchronized.OnError,
                synchronized.OnCompleted);

            var sampleObserver = Observer.Create<double>(
                timestamp =>
                {
                    var state = currentState;
                    if (state != null && state.Id < StateId.Annotation)
                    {
                        var output = new StateInfo();
                        output.Id = state.Id;
                        output.Trial = state.Trial;
                        output.ElapsedTime = timestamp;
                        synchronized.OnNext(state);
                    }
                },
                synchronized.OnError,
                synchronized.OnCompleted);
            return new CompositeDisposable(
                source.SubscribeSafe(stateObserver),
                sampler.SubscribeSafe(sampleObserver));
        });
    }
}
