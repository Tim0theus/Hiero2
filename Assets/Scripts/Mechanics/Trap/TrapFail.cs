using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrapAggregator))]
public class TrapFail : Activatable {

    TrapAggregator _trap;

    public override void Activate()
    {
        _trap.Solved();
    }

    public override void DeActivate()
    {
    }

    private void Awake()
    {
        _trap = GetComponent<TrapAggregator>();
    }

}
