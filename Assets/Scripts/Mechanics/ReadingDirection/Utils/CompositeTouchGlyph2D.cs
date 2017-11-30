using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompositeTouchGlyph2D : Riddle, IPointerDownHandler, IPointerUpHandler {
    private readonly List<TouchGlyph2D> _glyphs = new List<TouchGlyph2D>();

    private int _cursor;
    private Image _image;

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            Vector2 position = eventData.position;
            TouchGlyph2D touchedGlyph = null;
            float touchedGlyphDistance = float.MaxValue;

            foreach (TouchGlyph2D glyph in _glyphs) {
                if (glyph.IsInRegion(position)) {
                    float glyphDistance = glyph.DistanceSqrToCenter(position);
                    if (glyphDistance < touchedGlyphDistance) {
                        touchedGlyph = glyph;
                        touchedGlyphDistance = glyphDistance;
                    }
                }
            }

            if (touchedGlyph != null && !touchedGlyph.IsTouched) {
                if (touchedGlyph == _glyphs[_cursor]) {
                    touchedGlyph.Touch();
                    _cursor++;
                }
                else {
                    Failed();
                }
            }

            if (_cursor == _glyphs.Count) {
                Solved();
                _image.enabled = false;
            }
        }
    }

    public void Activate() {
        Reset();

        _cursor = 0;
        _image.enabled = true;
        enabled = true;

        foreach (TouchGlyph2D glyph in _glyphs) {
            glyph.Activate();
        }
    }

    public void DeActivate() {
        _image.enabled = false;
        enabled = false;

        foreach (TouchGlyph2D glyph in _glyphs) {
            glyph.DeActivate();
        }
    }

    private void Start() {
        _image = GetComponent<Image>();

        CenterComponent();

        enabled = false;
    }

    private void CenterComponent() {
        GetComponentsInChildren(true, _glyphs);

        Vector2 regionMax = Vector2.one * -float.MaxValue;
        Vector2 regionMin = Vector2.one * float.MaxValue;

        foreach (TouchGlyph2D glyph in _glyphs) {
            if (glyph.Region.UpperRight.x > regionMax.x)
                regionMax.x = glyph.Region.UpperRight.x;
            if (glyph.Region.UpperRight.y > regionMax.y)
                regionMax.y = glyph.Region.UpperRight.y;


            if (glyph.Region.LowerLeft.x < regionMin.x)
                regionMin.x = glyph.Region.LowerLeft.x;
            if (glyph.Region.LowerLeft.y < regionMin.y)
                regionMin.y = glyph.Region.LowerLeft.y;
        }

        Vector2 regionCenter = (regionMax - regionMin) / 2 + regionMin;
        Vector2 parentCenter = transform.parent.position;
        Vector2 centerOffset = regionCenter - parentCenter;

        foreach (TouchGlyph2D glyph in _glyphs) {
            glyph.UpdatePosition(centerOffset);
            glyph.DeActivate();
        }

        _image.rectTransform.offsetMax = (regionMax - regionCenter) / 2;
        _image.rectTransform.offsetMin = (regionMin - regionCenter) / 2;
    }
}