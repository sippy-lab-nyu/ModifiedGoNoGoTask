using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class StateInterval
{
    public IObservable<StateInfo> Process(IObservable<StateInfo> source)
    {
        return Observable.Create<StateInfo>(observer =>
        {
            var currentTrial = default(StateInfo);
            var currentState = default(StateInfo);
            var stateObserver = Observer.Create<StateInfo>(
                info =>
                {
                    if (currentState == null) currentState = info;
                    if (currentTrial == null || info.Trial != currentTrial.Trial)
                    {
                        currentTrial = info;
                    }

                    var output = new StateInfo();
                    output.Trial = currentState.Trial;
                    
                    if (info.Id < StateId.Annotation)
                    {
                        output.Id = currentState.Id;
                        output.ElapsedTime = info.ElapsedTime - currentState.ElapsedTime;
                        if (info.Id != currentState.Id)
                        {
                            observer.OnNext(output);
                            currentState = info;

                            output = new StateInfo();
                            output.Trial = info.Trial;
                            output.Id = info.Id;
                            output.ElapsedTime = 0;
                        }
                    }
                    else
                    {
                        output.Id = info.Id;
                        output.ElapsedTime = info.ElapsedTime - currentTrial.ElapsedTime;
                    }
                    observer.OnNext(output);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(stateObserver);
        });
    }
}
