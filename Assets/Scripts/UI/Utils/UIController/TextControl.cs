using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextControl : HighlightableUIControl {
    private Text _text;

    private Color NormalColor;

    private void Awake() {
        _text = GetComponent<Text>();

        NormalColor = _text.color;

        if (StartInactive) {
            DeActivate(0);
        }
    }

    private void Start() {
        _text.CrossFadeColor(InactiveColor, 0, false, true);
    }

    public override void Activate() {
        _text.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _text.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }

    public override void Highlight() {
        _text.CrossFadeColor(HighlightColor, FadeDuration, false, true);
    }

    public override void DeHighlight() {
        _text.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }


}
