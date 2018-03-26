using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenDoor : Activatable {

    public Animator anim;

    public override void Activate()
    {
        anim.SetTrigger("Activate");
    }

    public override void DeActivate()
    {
    }

}
