using UnityEngine;
using UnityEngine.UI;

public class Translator : UIControl, IEnableable {
    private Image _image;

    private DisplayMode _displayMode;

    private bool _isHidden;
    private bool _enabled;

    private Color NormalColor;

    private void Awake() {
        _image = GetComponent<Image>();
        NormalColor = _image.color;

        _image.CrossFadeColor(InactiveColor, 0, false, true);

        _isHidden = true;
    }

    private void UpdateView(object sender, ModeChangedEventArgs e) {
        _displayMode = e.Mode;
        _image.sprite = e.BeforeChange;

        Activate();
    }

    private void UpdateView(object sender, FocusChangedEventArgs e) {
        if (e.IsNewGlyph) {
            _isHidden = true;
            DeActivate();
        }
        else {
            UpdateView();
        }
    }

    private void UpdateView() {
        if (_image) _image.sprite = (_displayMode == DisplayMode.Hierglyph) ? LiteralPicker.Current.Transliteration : LiteralPicker.Current.Hieroglyph;

        if (_isHidden) {
            Activate();
        }
    }

    public override void Activate() {
        if (_enabled && _image != null) {
            _image.CrossFadeColor(NormalColor, FadeDuration, false, true);
        }
    }

    public override void DeActivate() {
        if (_image.sprite != null) {
            _image.CrossFadeColor(InactiveColor, FadeDuration, false, true);
        }
    }

    public void Enable() {
        LiteralPicker.OnFocusChanged += UpdateView;
        LiteralPicker.OnModeChanged += UpdateView;

        _enabled = true;
        Activate();
    }

    public void Disable() {
        LiteralPicker.OnFocusChanged -= UpdateView;
        LiteralPicker.OnModeChanged -= UpdateView;

        _enabled = false;
        DeActivate();
    }
}
