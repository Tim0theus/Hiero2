using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleControl : HighlightableUIControl {
    private Toggle _toggle;

    private Image _background;
    private Image _checkmark;

    private Color NormalColor;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();

        _background = transform.GetChild(0).GetComponent<Image>();
        _checkmark = _background.transform.GetChild(0).GetComponent<Image>();

        NormalColor = _checkmark.color;

        if (StartInactive)
        {
            DeActivate(0);
        }
    }

    public override void Activate() {
        _toggle.interactable = true;
        _background.CrossFadeColor(NormalColor, FadeDuration, false, true);

        if (_toggle.isOn) {
            _checkmark.CrossFadeColor(NormalColor, FadeDuration, false, true);
        }
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _toggle.interactable = false;
        _background.CrossFadeColor(InactiveColor, fadeDuration, false, true);

        _checkmark.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }

    public override void Highlight() {
        _background.CrossFadeColor(HighlightColor, FadeDuration, false, true);

        if (_toggle.isOn) {
            _checkmark.CrossFadeColor(HighlightColor, FadeDuration, false, true);
        }
    }

    public override void DeHighlight() {
        _background.CrossFadeColor(NormalColor, FadeDuration, false, true);

        if (_toggle.isOn)
        {
            _checkmark.CrossFadeColor(NormalColor, FadeDuration, false, true);
        }

    }


}
