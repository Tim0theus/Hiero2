using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum DisplayMode {
    Literal,
    Hierglyph,
    Random
}

public class PickerGlyph : Activatable {
    public ExtendedGlyph Glyph { get; private set; }

    public float Offset {
        set {
            _offset = value;
            Animate();
        }
    }

    public Color Color {
        set {
            _image.color = value;
            _fader.ActiveColor = value;
        }
    }

    public float Position {
        get { return _slot * _rectTransform.rect.width / 2 + _offset; }
    }

    public int Slot {
        get { return _slot; }
        private set {
            _slot = value;
            name = value.ToString();
        }
    }

    public EventHandler<ModeChangedEventArgs> OnModeChanged;

    private ModeChangedEventArgs _eventArgs;

    private RectTransform _parentRectTransform;
    private RectTransform _rectTransform;

    private DynamicTwoColorFader _fader;
    private Image _image;
    private Color _defaultColor;
    private bool _highlighted;

    private bool _inFocus;
    public bool _new = true;
    private float _time;

    private float _offset;
    private int _slot;

    private DisplayMode _newDisplayMode;
    private DisplayMode _currentDisplayMode;

    private readonly HashSet<DisplayMode> _availableModes = new HashSet<DisplayMode>();


    private void Start() {
        Animate();
        _image.color = _defaultColor.Transparent();
        _fader.Activate();

        _eventArgs = new ModeChangedEventArgs();
    }

    private void Update() {
        _inFocus = Math.Abs(Position) < 0.1;

        if (_inFocus) {
            if (_new) {
                //pulse animation
                _time += Time.deltaTime * 4;
                float scale = 1 + (float)Math.Sin(_time - Math.PI / 2) / 10 + 1 / 10f;
                float position = 0.5f + (float)Math.Sin(_time - Math.PI / 2) / 2;
                transform.localScale = Vector3.one * scale;
                transform.localPosition = Vector3.left * position * _rectTransform.rect.width;
            }
            //else {
            //    //scale back to normal
            //    float scale = transform.localScale.x;
            //    //if (scale > 0.1) {
            //    //    transform.localScale -= Vector3.one * scale * Time.deltaTime;
            //    //    transform.localPosition -= Vector3.right * 3 * transform.localPosition.x * Time.deltaTime;
            //    //}
            //}
        }
    }

    private void Animate() {
        float height = _parentRectTransform.rect.height / 2;
        float normalizedPosition = Mathf.Clamp(Position / height, -1, 1);
        float absoluteNormalizedPosition = Math.Abs(normalizedPosition);
        float invertedAbsoluteNormalizedPosition = 1 - absoluteNormalizedPosition;

        _rectTransform.localPosition = Vector2.up * (Position * invertedAbsoluteNormalizedPosition + height * normalizedPosition);

        _rectTransform.localScale = new Vector3(1, invertedAbsoluteNormalizedPosition, 1);

        if (_highlighted) {
            Color = Global.Colors.HighlightYellow;
        }
        else {
            Color color = _defaultColor;
            color.a = invertedAbsoluteNormalizedPosition * 1.25f;
            Color = color;
        }
    }

    public static PickerGlyph Create(ExtendedGlyph glyph, int slot, Transform parent, DisplayMode displayMode = DisplayMode.Literal, bool newGlyph = true) {
        GameObject pickerGlyphObject = new GameObject(slot.ToString());

        PickerGlyph pickerGlyph = pickerGlyphObject.AddComponent<PickerGlyph>();
        pickerGlyph.transform.SetParent(parent, false);

        pickerGlyph._parentRectTransform = parent.transform.GetComponent<RectTransform>();

        RectTransform rectTransform = pickerGlyphObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

        AspectRatioFitter aspectRatioFitter = pickerGlyphObject.AddComponent<AspectRatioFitter>();
        aspectRatioFitter.aspectRatio = 1;
        aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;

        Image image = pickerGlyphObject.AddComponent<Image>();

        if (glyph.Hieroglyph) {
            pickerGlyph._availableModes.Add(DisplayMode.Hierglyph);
        }
        if (glyph.Transliteration) {
            pickerGlyph._availableModes.Add(DisplayMode.Literal);
        }

        if (glyph.Transliteration != null) {
            switch (displayMode) {
                case DisplayMode.Literal:
                    image.sprite = glyph.Transliteration;
                    break;
                case DisplayMode.Hierglyph:
                    image.sprite = glyph.Hieroglyph;
                    break;
                default:
                    float random = Random.value;
                    image.sprite = random < 0.5f ? glyph.Transliteration : glyph.Hieroglyph;
                    break;
            }
        }
        else {
            image.sprite = glyph.Hieroglyph;
        }

        image.material = Resources.Load<Material>("Fonts/Nilus");
        image.raycastTarget = false;

        DynamicTwoColorFader fader = DynamicTwoColorFader.Create(image);
        fader.ActiveColor = Global.Colors.PickerGlyph;
        fader.DeactiveColor = Global.Colors.PickerGlyph.Transparent();

        pickerGlyph._rectTransform = rectTransform;
        pickerGlyph._image = image;
        pickerGlyph._fader = fader;
        pickerGlyph._defaultColor = Global.Colors.PickerGlyph;
        pickerGlyph._currentDisplayMode = displayMode;

        pickerGlyph.Slot = slot;
        pickerGlyph.Glyph = glyph;

        pickerGlyph._new = newGlyph;

        return pickerGlyph;
    }

    public void SetMode(DisplayMode displayMode) {
        if (!_availableModes.Contains(displayMode) || _currentDisplayMode == displayMode) return;

        _newDisplayMode = displayMode;

        _fader.DeActivate(ChangeSprite);
    }

    private void ChangeSprite(object sender, EventArgs e) {
        _eventArgs.BeforeChange = _image.sprite;

        switch (_newDisplayMode) {
            case DisplayMode.Literal:
                _image.sprite = Glyph.Transliteration;
                break;
            default:
                _image.sprite = Glyph.Hieroglyph;
                break;
        }

        transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
        transform.localScale = Vector3.one;

        _eventArgs.AfterChange = _image.sprite;
        _eventArgs.Mode = _newDisplayMode;
        _fader.Activate(ModeChangeFinished);

        _currentDisplayMode = _newDisplayMode;
    }

    private void ModeChangeFinished(object sender, EventArgs e) {
        if (OnModeChanged != null) {
            OnModeChanged(null, _eventArgs);
        }
    }

    public override void Activate() {
        _fader.Activate();
    }

    public override void DeActivate() {
        _fader.DeActivate();
    }

    public void Highlight() {
        _highlighted = true;
    }

    public void DeHighlight() {
        _highlighted = false;
    }

    public void Touch() {
        if (_new)
            _new = false;
        else if (_currentDisplayMode == DisplayMode.Literal) TabPicker.instance.Open();
    }

    public void Remove() {
        _fader.DeActivated += Destroy;
        _fader.DeActivate();
    }

    private void Destroy(object sender, EventArgs e) {
        Destroy(gameObject);
    }

    public void Move(int numberOfSlots) {
        Slot += numberOfSlots;
    }
}

public class ModeChangedEventArgs : EventArgs {
    public Sprite BeforeChange { get; set; }
    public Sprite AfterChange { get; set; }
    public DisplayMode Mode { get; set; }
}