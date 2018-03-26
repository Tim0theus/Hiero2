using UnityEngine;


public class BreakableReed : Breakable {
    public bool Highlight;
    public MeshRenderer MeshRenderer;

    private Fader _fader;

    private new void Start() {
        base.Start();

        if (Highlight && MeshRenderer != null) {
            _fader = MaterialFader.Create(MeshRenderer, new Color(0.2f, 0.2f, 0.2f), Color.black, true, 1, true);
            _fader.Activate();
        }
    }

    public override void Break(bool silent = false) {
        base.Break(silent);

        if (Highlight) {
            _fader.DeActivate();
        }
    }
}