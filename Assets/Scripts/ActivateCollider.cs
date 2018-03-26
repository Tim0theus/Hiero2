using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCollider : Activatable {

    public override void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }

    public override void DeActivate()
    {
        GetComponent<Collider>().enabled = false;
    }

}
