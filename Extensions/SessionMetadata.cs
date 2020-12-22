using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Source)]
public class SessionMetadata
{
    public double JoystickThreshold { get; set; }

    public double NoGoThreshold { get; set; }

    public IObservable<SessionMetadata> Process()
    {
        return Observable.Return(this);
    }

    override 
}
