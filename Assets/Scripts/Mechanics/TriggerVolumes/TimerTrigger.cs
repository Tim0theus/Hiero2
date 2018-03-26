using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrigger : TriggerVolume {
    protected override void Activate()
    {
        Timer.instance.Activate();
    }

    protected override void Check()
    {
    }

    protected override void Deactivate()
    {
        Timer.instance.DeActivate();
    }
}
