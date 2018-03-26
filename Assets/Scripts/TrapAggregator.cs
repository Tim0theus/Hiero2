using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAggregator : RiddleAggregator {

    public new void Solved()
    {
        _failed = true;
        SoundController.instance.Play("error");
        Notify(NotificationDelay);
        Indicate(IndicationDelay);
        FillPicker();

        Reset();
    }

    public override void Solve()
    {
        Solved();
    }

    public override void Reset()
    {
        base.Reset();
        Indicate(IndicationDelay);
    }


    public override void UpdateStatus(IObservable observable)
    {
        Riddle riddle = (Riddle)observable;

        if (riddle.IsFailed)
        {
            Solved();
        }

    }
}
