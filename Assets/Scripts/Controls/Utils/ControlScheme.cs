using UnityEngine;

public abstract class ControlScheme : MonoBehaviour, IActivatable {
    public Vector2 LookVector;
    public Vector2 MoveVector;

    public InputControlElement LookControl;
    public InputControlElement MoveControl;

    public virtual void Activate() {
        ActivateLookControl();
        ActivateMoveControl();

        enabled = true;
    }

    public virtual void DeActivate() {
        DeActivateLookControl();
        DeActivateMoveControl();

        enabled = false;
    }

    private void ActivateLookControl() {
        LookControl.Activate();
    }

    private void DeActivateLookControl() {
        LookControl.DeActivate();
        LookVector = Vector2.zero;
    }

    private void ActivateMoveControl() {
        MoveControl.Activate();
    }

    private void DeActivateMoveControl() {
        MoveControl.DeActivate();
        MoveVector = Vector2.zero;
    }
}
