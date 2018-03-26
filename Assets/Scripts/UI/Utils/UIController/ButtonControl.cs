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
        _button.enabled = true;
        _image.raycastTarget = true;

        _button.targetGraphic.raycastTarget = true;
        _button.targetGraphic.canvasRenderer.SetAlpha(1);

        if (_button.interactable)
        {
            _image.CrossFadeColor(_button.colors.normalColor, FadeDuration, false, true);
            _text.CrossFadeColor(_button.colors.normalColor, FadeDuration, false, true);
        }
        else
        {
            _image.CrossFadeColor(_button.colors.disabledColor, FadeDuration, false, true);
            _text.CrossFadeColor(_button.colors.disabledColor, FadeDuration, false, true);
        }
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _button.enabled = false;
        _image.raycastTarget = false;

        _button.targetGraphic.raycastTarget = false;
        _button.targetGraphic.canvasRenderer.SetAlpha(0);

        _image.CrossFadeColor(InactiveColor, fadeDuration, false, true);
        _text.CrossFadeColor(InactiveColor, fadeDuration, false, true);

    }

    public override void Highlight() {
        _text.CrossFadeColor(_button.colors.highlightedColor, FadeDuration, false, true);
    }

    public override void DeHighlight() {
        _text.CrossFadeColor(_button.colors.normalColor, FadeDuration, false, true);
    }

    public void Click() {
        _button.onClick.Invoke();
    }
}