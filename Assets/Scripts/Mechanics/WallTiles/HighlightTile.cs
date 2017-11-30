using UnityEngine;

public class HighlightTile : FaderActivatable {

    private void Start() {
        Fader = MaterialFader.Create(GetComponent<Renderer>(), Global.Colors.HighlightYellow, Color.black, true);
    }
}
