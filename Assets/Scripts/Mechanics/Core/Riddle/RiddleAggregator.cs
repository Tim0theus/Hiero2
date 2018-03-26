using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnlockOrder {
    OutOfOrder,
    InOrder
}

public class RiddleAggregator : Riddle, IObserver {
    public UnlockOrder RiddleOrder = UnlockOrder.OutOfOrder;
    public List<Riddle> Riddles = new List<Riddle>();

    protected readonly HashSet<Riddle> _riddles = new HashSet<Riddle>();
    protected int _riddleCursor;

    protected int SolvedElements {
        get {
            int count = 0;
            foreach (Riddle r in _riddles)
            {
                if (r.IsSolved) count++;
            }

            return count; }
    }

    protected void Awake() {
        foreach(Riddle riddle in Riddles) {
            if(riddle != null)
                Add(riddle);
        }

        if(RiddleOrder == UnlockOrder.InOrder) {
            for(int r = 1; r < Riddles.Count; r++) {
                Riddles[r].Disable();
            }
        }
    }

    public override void Reset() {
        base.Reset();
        foreach (Riddle rid in Riddles)
        {
            rid.Reset();
        }
    }

    public override void Solve()
    {
        foreach (Riddle rid in Riddles)
        {
            rid.Solve();
        }
    }

    public override void Disable()
    {
        foreach (Riddle rid in Riddles)
        {
            rid.Disable();
        }
        base.Disable();
    }

    public override void Enable()
    {
        foreach (Riddle rid in Riddles)
        {
            rid.Enable();
        }
        base.Enable();
    }


    public new void Failed()
    {
        base.Failed();
        foreach(Riddle rid in Riddles)
        {
            GameControl.instance.SubtractPoint(null, null);
        }
        if (SoundController.instance) SoundController.instance.Play("error");
        Reset();
    }

    public virtual void UpdateStatus(IObservable observable) {
        Riddle riddle = (Riddle)observable;

        if(RiddleOrder == UnlockOrder.InOrder && _riddleCursor < Riddles.Count - 1) {
            if(riddle != Riddles[_riddleCursor]) return;

            _riddleCursor++;
            Riddles[_riddleCursor].Enable();
        }

        if (SolvedElements == _riddles.Count)
        {
            Solved();
        }
        else
        {
            int temp = 0;
            foreach (Riddle r in Riddles)
            {
                temp += r.IsSolved || r.IsFailed ? 1 : 0;
            }
            if (temp == _riddles.Count && temp != SolvedElements) Failed();
        }

    }

    public void Add(IObservable observable) {
        _riddles.Add(observable as Riddle);
        observable.Subscribe(this);
    }

    public void Remove(IObservable observable) {
        _riddles.Remove(observable as Riddle);
        observable.Unsubscribe(this);
    }
}