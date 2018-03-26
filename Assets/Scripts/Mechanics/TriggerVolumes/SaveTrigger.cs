using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : TriggerVolume
{
    protected override void Activate()
    {
        GameControl.instance.ActivateSave();
    }

    protected override void Check()
    {
    }

    protected override void Deactivate()
    {
        GameControl.instance.DeactivateSave();
    }
}
