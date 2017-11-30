using UnityEngine;

public enum State {
    Nothing,
    Activate,
    Deactivate
}

[RequireComponent(typeof(Collider))]
public abstract class TriggerVolume : MonoBehaviour {
    public bool OneWay;
    public bool Once;

    public State OnEnter;
    public State OnStay;
    public State OnExit;

    private Vector3 _center;
    private Vector3 _entryPosition;
    private Vector3 _entryDireciton;
    private Vector3 _exitDirection;

    private Collider _collider;

    private void Reset() {
        gameObject.layer = LayerMask.NameToLayer("Volumes");
    }

    private void Awake() {
        _collider = GetComponent<Collider>();
        _center = _collider.bounds.center;
    }

    private void OnTriggerEnter(Collider other) {
        if (OneWay) {
            _entryPosition = other.transform.position;
            _entryDireciton = (_center - _entryPosition).normalized;
        }

        switch (OnEnter) {
            case State.Activate:
                Activate();
                break;
            case State.Deactivate:
                Deactivte();
                break;
        }
    }

    private void OnTriggerStay(Collider other) {
        switch (OnStay) {
            case State.Activate:
                Activate();
                break;
            case State.Deactivate:
                Deactivte();
                break;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (OneWay) {
            _exitDirection = (other.transform.position - _entryPosition).normalized;
            float exitAngle = Vector3.Dot(_exitDirection, _entryDireciton);

            if (exitAngle < 0.8f) return;
        }

        switch (OnExit) {
            case State.Activate:
                Activate();
                break;
            case State.Deactivate:
                Deactivte();
                break;
        }

        if (Once) { _collider.enabled = false; }
    }

    protected abstract void Activate();
    protected abstract void Deactivte();

}


