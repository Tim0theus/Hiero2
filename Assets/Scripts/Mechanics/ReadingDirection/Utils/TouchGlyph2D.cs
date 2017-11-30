using System;
using UnityEngine;
using UnityEngine.UI;

public class TouchGlyph2D : FaderActivatable {
    public RectangularRegion Region { get; private set; }
    public bool IsTouched { get; private set; }

    private readonly Color _color = Global.Colors.GlyphWhite;
    private readonly Color _solvedColor = Global.Colors.HighlightYellow;

    public bool IsInRegion(Vector2 position) {
        return Region.IsInside(position);
    }

    public float DistanceSqrToCenter(Vector2 position) {
        return (position - Region.Center).sqrMagnitude;
    }

    public void UpdatePosition(Vector3 offset) {
        transform.position -= offset;
        Region.SetCenter(transform.position);
    }

    public void Touch() {
        IsTouched = true;
        Fader.ActivateFormPoint(1);
    }

    public override void Activate() {
        IsTouched = false;
        Fader.Activate();
        enabled = true;
    }

    public override void DeActivate() {
        Fader.DeActivate();
        enabled = false;
    }

    private void Start() {
        Image image = GetComponent<Image>();
        image.raycastTarget = false;
        Fit glyphFit = ResourceLoader.Get(image.sprite.name).Fit;

        CreateRegion(glyphFit);

        Fader = ImageFader.Create(image, _color);
        Fader.AddPoint(_solvedColor, 4);
    }

    private void CreateRegion(Fit glyphFit) {
        float smallSize = Screen.height / 8f / 2f;

        float regionWidth = glyphFit == Fit.Small || glyphFit == Fit.Tall ? smallSize / 2 : smallSize;
        float regionHeight = glyphFit == Fit.Small || glyphFit == Fit.Flat ? smallSize / 2 : smallSize;

        regionWidth *= Math.Abs(transform.lossyScale.x);
        regionHeight *= Math.Abs(transform.lossyScale.y);

        Region = new RectangularRegion(transform.position, regionWidth, regionHeight);
    }
}