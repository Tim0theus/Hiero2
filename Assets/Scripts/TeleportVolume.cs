using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportVolume : TriggerVolume {

    public GameObject target;
    public GameObject player;

    public FullscreenOverlay overlay;

    protected override void Activate()
    {
        player.transform.position = target.transform.position;
        overlay.Activate();
        overlay.DeActivate();
    }

    protected override void Check()
    {
    }

    protected override void Deactivate()
    {
    }

}
