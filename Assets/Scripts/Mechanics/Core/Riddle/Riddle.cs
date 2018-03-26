using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Riddle : MonoBehaviour, IObservable {
    public float NotificationDelay;
    public float IndicationDelay;

    public List<Texture2D> GlyphsForPicker = new List<Texture2D>();
    public List<Activatable> Indicators = new List<Activatable>();

    public EventHandler onSolved;
    public EventHandler onFailed;
    public EventHandler onReset;

    private readonly HashSet<RiddleAggregator> _riddleBases = new HashSet<RiddleAggregator>();

    public bool IsSolved {
        get {
            return _solved;
        }
    }

    public bool IsFailed
    {
        get
        {
            return _failed;
        }
    }

    public virtual void Solve()
    {
        Solved();
    }

    protected bool _solved;
    protected void Solved() {
        if (!_solved) {
            _solved = true;
            _failed = false;
            if (onSolved != null) onSolved(this, null);
            Notify(NotificationDelay);
            Indicate(IndicationDelay);
            FillPicker();
        }
    }

    protected bool _failed;
    protected void Failed() {
        if (!_failed) {
            _solved = false;
            _failed = true;
            if (onFailed != null) onFailed(this, null);
            Notify();
        }
    }


    protected void Notify(float delayInSeconds = 0) {
        if (delayInSeconds > 0) {
            StartCoroutine(DelayedNotify(delayInSeconds));
        }
        else {
            foreach (IObserver riddleBase in _riddleBases) {
                riddleBase.UpdateStatus(this);
            }
        }
    }

    private IEnumerator DelayedNotify(float seconds) {
        yield return new WaitForSeconds(seconds);

        foreach (IObserver riddleBase in _riddleBases) {
            riddleBase.UpdateStatus(this);
        }
    }

    protected void Indicate(float delayInSeconds) {
        if (delayInSeconds > 0) {
            StartCoroutine(DelayedIndicate(delayInSeconds));
        }
        else {
            foreach (IActivatable indicator in Indicators) {
                indicator.Activate();
            }
        }
    }

    private IEnumerator DelayedIndicate(float seconds) {
        yield return new WaitForSeconds(seconds);

        foreach (IActivatable indicator in Indicators) {
            indicator.Activate();
        }
    }

    protected void FillPicker() {
        foreach (Texture2D glyph in GlyphsForPicker) {
            LiteralPicker.AddNewGlyph(glyph.name);
        }
    }

    public void Subscribe(IObserver observer) {
        _riddleBases.Add(observer as RiddleAggregator);
    }

    public void Unsubscribe(IObserver observer) {
        _riddleBases.Remove(observer as RiddleAggregator);
    }

    public virtual void Enable() {
        enabled = true;
    }

    public virtual void Disable() {
        enabled = false;
    }

    public virtual void Reset() {
        _failed = false;
        _solved = false;
        if (onReset != null) onReset(this, null);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Riddle), true)]
    [CanEditMultipleObjects]
    public class RiddleEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (Application.isPlaying) {
                if (GUILayout.Button("Solve")) {

                    foreach (UnityEngine.Object o in targets) {

                        Riddle riddle = (Riddle)o;
                        riddle.Solved();
                    }
                }
                if (GUILayout.Button("Unsolve"))
                {
                    foreach (UnityEngine.Object o in targets)
                    {

                        Riddle riddle = (Riddle)o;
                        riddle.Failed();
                    }
                }
            }
        }
    }
#endif
}
