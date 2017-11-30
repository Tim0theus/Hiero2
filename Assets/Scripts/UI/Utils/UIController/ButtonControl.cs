using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonControl : HighlightableUIControl {
    private Button _button;

    private Image _image;
    private Text _text;

    private void Awake() {
        _button = GetComponent<Button>();

        _image = GetComponent<Image>();
        _text = transform.GetChild(0).GetComponent<Text>();

        if (StartInactive) {
            DeActivate(0);
        }
    }

    public override void Activate() {
        _button.interactable = true;
        _image.raycastTarget = true;

        _image.CrossFadeColor(NormalColor, FadeDuration, false, true);
        _text.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _button.interactable = false;
        _image.raycastTarget = false;

        _image.CrossFadeColor(InactiveColor, fadeDuration, false, true);
        _text.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }

    public override void Highlight() {
        _text.CrossFadeColor(HighlightColor, FadeDuration, false, true);
    }

    public override void DeHighlight() {
        _text.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }

    public void Click() {
        _button.onClick.Invoke();
    }
}