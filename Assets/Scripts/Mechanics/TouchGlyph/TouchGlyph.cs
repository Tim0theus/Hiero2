using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
#endif

[RequireComponent(typeof(MeshRenderer))]
public class TouchGlyph : Riddle, IActivatable, IPointerDownHandler, IPointerUpHandler {
    private string _requiredGlyph;
    private Collider _collider;
    private Fader _fader;

    public void Activate() {
        _fader.Activate();
        _collider.enabled = true;
    }

    public void DeActivate() {
        _fader.DeActivate();
        Reset();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (_requiredGlyph != LiteralPicker.Current.GlyphCode) return;

        if (eventData.button == PointerEventData.InputButton.Left) {
            Activate();
            Solved();
            _collider.enabled = false;
        }
    }

    private void Start() {
        Renderer renderer = GetComponent<Renderer>();
        _requiredGlyph = renderer.ExtractGlyphName();

        _collider = GetComponent<Collider>();

        _fader = MaterialFader.Create(renderer, Global.Colors.HighlightYellow, Color.black, true);
    }
}