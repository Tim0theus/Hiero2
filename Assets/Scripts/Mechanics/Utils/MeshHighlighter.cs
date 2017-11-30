using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshHighlighter : FaderActivatable {

    private void Start() {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Fader = MaterialFader.Create(meshRenderer, new Color(0.2f, 0.2f, 0.2f), Color.black, true, 1, true);
        Fader.Activate();
    }
}
