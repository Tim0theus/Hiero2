using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DropdownControl : HighlightableUIControl {
    private Dropdown _dropdown;

    private Image _background;
    private Text _text;

    private Color NormalColor;

    private void Awake() {
        _dropdown = GetComponent<Dropdown>();
        _text = transform.Find("Label").GetComponent<Text>();
        _background = transform.Find("Background").GetComponent<Image>();

        NormalColor = _text.color;

        if (StartInactive) {
            DeActivate(0);
        }
    }

    //private void Start() {
    //    _text.CrossFadeColor(InactiveColor, 0, false, true);
    //    _background.CrossFadeColor(InactiveColor, 0, false, true);
    //}

    public override void Activate() {
        _dropdown.interactable = true;
        _background.raycastTarget = true;

        _text.CrossFadeColor(NormalColor, FadeDuration, false, true);
        _background.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _dropdown.interactable = false;
        _background.raycastTarget = false;

        _text.CrossFadeColor(InactiveColor, fadeDuration, false, true);
        _background.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }

    public override void Highlight() {
        _background.CrossFadeColor(HighlightColor, FadeDuration, false, true);
    }

    public override void DeHighlight() {
        _background.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }
}