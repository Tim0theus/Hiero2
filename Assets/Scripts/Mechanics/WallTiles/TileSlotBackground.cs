using UnityEngine;

public class TileSlotBackground : FaderActivatable, IEnableable {

    private void Awake() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Fader = MaterialFader.Create(renderer, Color.black);
    }

    public void Enable() {
        Activate();
    }

    public void Disable() {
        DeActivate();
    }
}
