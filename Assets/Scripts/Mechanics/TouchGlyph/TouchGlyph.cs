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

    public override void Reset()
    {
        base.Reset();
        _collider.enabled = true;
        _fader.DeActivate();
    }

    public override void Solve()
    {
        base.Solve();
        Activate();
        _collider.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (LiteralPicker.Current.GlyphCode != _requiredGlyph)
            {
                SoundController.instance.Play("error");
                Failed();
                Reset();
                return;
            }


            Activate();
            Solved();
            _collider.enabled = false;
        }
    }

    private void Awake() {
        Renderer renderer = GetComponent<Renderer>();
        _requiredGlyph = renderer.ExtractGlyphName();

        _collider = GetComponent<Collider>();

        _fader = MaterialFader.Create(renderer, Global.Colors.HighlightYellow, Color.black, true);
    }
}