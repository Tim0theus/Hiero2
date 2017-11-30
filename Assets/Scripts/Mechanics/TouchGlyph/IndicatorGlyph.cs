
using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class IndicatorGlyph : FaderActivatable {
    public bool FadeIn;
    public bool StayVisible;
    public bool StartActive;

    private void Start() {
        Renderer meshRenderer = GetComponent<MeshRenderer>();

        EventHandler fadeout = null;
        if (!StayVisible) {
            fadeout = FadeOut;
        }

        if (FadeIn) {
            Fader = MaterialFader.Create(meshRenderer, Global.Colors.HighlightYellow, true, fadeout);
        }
        else {
            Fader = MaterialFader.Create(meshRenderer, Global.Colors.HighlightYellow, Color.black, true);
        }

        if (StartActive) {
            Fader.Activate();
        }
    }

    private void FadeOut(object sender, EventArgs e) {
        Fader.DeActivate();
    }
}
