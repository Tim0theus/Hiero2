using UnityEngine;

public abstract class Activatable : MonoBehaviour, IActivatable {
    public abstract void Activate();
    public abstract void DeActivate();
}

