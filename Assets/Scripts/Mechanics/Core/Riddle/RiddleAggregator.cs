using System;
using System.Collections.Generic;

public enum UnlockOrder {
    OutOfOrder,
    InOrder
}

public class RiddleAggregator : Riddle, IObserver {
    public UnlockOrder RiddleOrder = UnlockOrder.OutOfOrder;
    public List<Riddle> Riddles = new List<Riddle>();

    private readonly HashSet<Riddle> _riddles = new HashSet<Riddle>();
    private int _riddleCursor;
    private int _solvedElements;

    protected int SolvedElements {
        get { return _solvedElements; }
        private set { _solvedElements = Math.Min(Math.Max(0, value), _riddles.Count); }
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

    protected new void Reset() {
        base.Reset();
        _solvedElements = 0;
    }

    public virtual void UpdateStatus(IObservable observable) {
        Riddle riddle = (Riddle)observable;

        if(RiddleOrder == UnlockOrder.InOrder && _riddleCursor < Riddles.Count - 1) {
            if(riddle != Riddles[_riddleCursor]) return;

            _riddleCursor++;
            Riddles[_riddleCursor].Enable();
        }

        SolvedElements += riddle.Status.Solved ? 1 : 0;
        SolvedElements -= riddle.Status.Failure ? 1 : 0;

        if(SolvedElements == _riddles.Count) {
            Solved();
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