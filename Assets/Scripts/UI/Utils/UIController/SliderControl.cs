using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderControl : HighlightableUIControl, IDragHandler {
    private Slider _slider;

    private Image _handle;
    private Image _fill;
    private Image _background;

    private bool _highlight;

    private Color NormalColor;

    private void Awake() {
        _slider = GetComponent<Slider>();

        _handle = transform.Find("Handle Slide Area").GetChild(0).GetComponent<Image>();
        _fill = transform.Find("Fill Area").GetChild(0).GetComponent<Image>();
        _background = transform.Find("Background").GetComponent<Image>();

        NormalColor = _fill.color;

        if (StartInactive) {
            DeActivate(0);
        }
    }

    //private void Start() {
    //    _fill.CrossFadeColor(InactiveColor, 0, false, true);
    //    _background.CrossFadeColor(InactiveColor, 0, false, true);
    //}

    public override void Activate() {
        _slider.interactable = true;
        _handle.raycastTarget = true;
        _background.raycastTarget = true;

        _fill.CrossFadeColor(NormalColor, FadeDuration, false, true);
        _background.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _slider.interactable = false;
        _handle.raycastTarget = false;
        _background.raycastTarget = false;

        _fill.CrossFadeColor(InactiveColor, fadeDuration, false, true);
        _background.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }


    public override void Highlight() {
        if (!_highlight) {
            _fill.CrossFadeColor(HighlightColor, FadeDuration, false, true);
            _background.CrossFadeColor(HighlightColor, FadeDuration, false, true);
            _highlight = true;
        }
    }

    public override void DeHighlight() {
        _fill.CrossFadeColor(NormalColor, FadeDuration, false, true);
        _background.CrossFadeColor(NormalColor, FadeDuration, false, true);
        _highlight = false;
    }



    public void OnDrag(PointerEventData eventData) {
        Highlight();
    }
}
