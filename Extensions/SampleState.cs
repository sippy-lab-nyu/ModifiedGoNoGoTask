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
    public IObservable<StateInfo> Process(IObservable<StateInfo> source)
    {
        return Observable.Create<StateInfo>(observer =>
        {
            var currentState = default(StateInfo);
            var baseTime = HighResolutionScheduler.Now;
            var synchronized = Observer.Synchronize(observer);
            var stateObserver = Observer.Create<StateInfo>(
                state =>
                {
                    synchronized.OnNext(state);
                    if (currentState.Id < StateId.Annotation)
                    {
                        baseTime = HighResolutionScheduler.Now;
                        currentState = state;
                    }
                },
                synchronized.OnError,
                synchronized.OnCompleted);

            var sampleTimer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(50));
            var sampleObserver = Observer.Create<long>(
                tick =>
                {
                    var state = currentState;
                    if (state != null)
                    {
                        var deltaTime = HighResolutionScheduler.Now - baseTime;
                        var output = new StateInfo();
                        output.Id = state.Id;
                        output.Trial = state.Trial;
                        output.ElapsedTime = state.ElapsedTime + deltaTime.TotalSeconds;
                        synchronized.OnNext(output);
                    }
                },
                synchronized.OnError,
                synchronized.OnCompleted);
            return new CompositeDisposable(
                source.SubscribeSafe(stateObserver),
                sampleTimer.SubscribeSafe(sampleObserver));
        });
    }
}
