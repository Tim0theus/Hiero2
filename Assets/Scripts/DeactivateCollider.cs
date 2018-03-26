using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateCollider : Activatable
{
    public override void Activate()
    {
        GetComponent<Collider>().enabled = false;
    }

    public override void DeActivate()
    {
        GetComponent<Collider>().enabled = true;
    }
}
	
