using UnityEngine;

public abstract class InputControlElement : MonoBehaviour, IActivatable {
    public Vector2 Vector;
    public abstract void Activate();
    public abstract void DeActivate();
}