using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LiteralPicker : Activatable, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler {
    public Image Background;
    public Image Highlight;
    [Header("Sounds")]
    public AudioClip AddSound;
    public AudioClip FocusChangeSound;

    public static ExtendedGlyph Current {
        get { return _current && !_newGlyph ? _current.Glyph : new ExtendedGlyph(); }
    }

    private const float PitchShift = 0.15f;
    private static readonly Color BackgroundColor = new Color(0, 0, 0, 0.3f);

    public static EventHandler<FocusChangedEventArgs> OnFocusChanged;
    public static EventHandler<ModeChangedEventArgs> OnModeChanged;

    private static readonly FocusChangedEventArgs FocusChangedEventArgs = new FocusChangedEventArgs();

    public static LiteralPicker instance;
    private static int _instanceId;

    private static PickerGlyph _current;
    private static PickerGlyph _last;

    private static Transform _transform;
    private static RectTransform _rectTransform;

    public static readonly Dictionary<string, PickerGlyph> PickerGlyphs = new Dictionary<string, PickerGlyph>();
    private static readonly Queue<PickerGlyph> NewGlyphs = new Queue<PickerGlyph>();

    private static float _absoluteOffset;
    private static float _inputOffset;
    private static float _toCenterOffset;
    private static float _toFocusOffset;

    private static AudioSource _audioSource;
    private static AudioClip _addSound;

    private static Image _highlightImage;
    private static Image _background;
    private Image _touchArea;

    private static FullscreenOverlay _overlay;
    private static AnimatorActivatable _animatorActivatable;

    private static bool _animate;
    private static bool _newGlyph;

    private static bool _beginClick;

    public void OnDrag(PointerEventData eventData) {
        if (!_newGlyph) {
            _inputOffset = eventData.delta.y / 2;

            Animate();

            _inputOffset = 0;
        }
        _beginClick = false;
    }

    public void OnEndDrag(PointerEventData eventData) {
        _animate = true;
        _beginClick = false;
    }


    public void OnPointerDown(PointerEventData eventData) {
        _beginClick = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (_beginClick && _current && _current.enabled) {
            _current.Touch();
            _current.SetMode(DisplayMode.Literal);
        }
    }

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _instanceId = GetInstanceID();

        OnFocusChanged += FocusChanged;

        _transform = transform;
        _rectTransform = transform.GetComponent<RectTransform>();

        _audioSource = GetComponent<AudioSource>();
        _addSound = AddSound;

        _highlightImage = Highlight;
        _background = Background;
        _touchArea = GetComponent<Image>();

        _background.color = Color.black;
        _background.CrossFadeAlpha(0, 0, false);

        _overlay = transform.parent.GetComponentInChildren<FullscreenOverlay>();
        _animatorActivatable = _highlightImage.GetComponent<AnimatorActivatable>();
        _animatorActivatable.OnAnimationFinished += AddAnimationFinished;
        PickerGlyphs.Clear();
    }

    private void Start() {
        DeActivate();
    }

    public override void Activate() {
        if (PickerGlyphs.Count > 0) {
            _background.CrossFadeColor(BackgroundColor, 1, false, true);

            enabled = true;
            _touchArea.enabled = true;

            foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values) {
                pickerGlyph.Activate();
            }
        }
    }

    public override void DeActivate() {
        _current = null;

        _background.CrossFadeColor(BackgroundColor.Transparent(), 1, false, true);

        enabled = false;
        _touchArea.enabled = false;

        foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values) {
            pickerGlyph.DeActivate();
        }
    }

    private void Update() {
        if (_animate) {
            Animate();

            if (Math.Abs(_toFocusOffset) > 0.1) {
                float reducer = _toFocusOffset * Time.deltaTime * 5;
                _inputOffset = -reducer;
                _toFocusOffset -= reducer;
            }
            else if (Math.Abs(_toCenterOffset) > 0.1) {
                _inputOffset = -_toCenterOffset / 5;
            }
            else {
                _inputOffset = _toCenterOffset = _toFocusOffset = 0;
                _animate = false;
            }
        }
    }

    private static void Animate() {
        _toCenterOffset = ushort.MaxValue;
        _absoluteOffset += _inputOffset;

        foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values) {
            pickerGlyph.Offset = _absoluteOffset;

            if (Math.Abs(pickerGlyph.Position) < Math.Abs(_toCenterOffset)) {
                _toCenterOffset = pickerGlyph.Position;
                _current = pickerGlyph;
            }
        }

        if (PickerGlyphs.Count > 1) {
            if (_current != _last) {
                _current.Highlight();
                _last.DeHighlight();
                _last = _current;

                _audioSource.pitch = _inputOffset > 0 ? 1 - PitchShift : 1 + PitchShift;

                if (OnFocusChanged != null) OnFocusChanged(null, FocusChangedEventArgs);
            }
        }
    }

    public static string[] SaveGlyphs()
    {
        string[] arr = new string[PickerGlyphs.Count];

        foreach(var k in PickerGlyphs)
        {
            arr[k.Value.Slot] = k.Key;
        }

        return arr;
    }

    public static void LoadGlyphs(string[] glyphs)
    {
        int slot = 0;

        if (glyphs.Length < 1) return;

        ExtendedGlyph glyph = ResourceLoader.Get(glyphs[0]);
        PickerGlyph newPickerGlyph = PickerGlyph.Create(glyph, slot++, _transform, DisplayMode.Literal, false);
        PickerGlyphs.Add(glyphs[0], newPickerGlyph);
        _current = newPickerGlyph;
        _last = newPickerGlyph;

        newPickerGlyph.Highlight();
        newPickerGlyph.Color = Global.Colors.HighlightYellow;

        _absoluteOffset += _rectTransform.rect.width / 2;

        instance.Activate();

        for (int i = 1; i < glyphs.Length; i++)
        {
            glyph = ResourceLoader.Get(glyphs[i]);
            newPickerGlyph = PickerGlyph.Create(glyph, slot++, _transform, DisplayMode.Literal, false);
            PickerGlyphs.Add(glyphs[i], newPickerGlyph);
            newPickerGlyph.Activate();
        }

        foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values)
        {
            pickerGlyph.Offset = _absoluteOffset;
        }

        FocusOnGlyph(glyph.GlyphCode);

    }

    public static void AddNewGlyph(string glyphCode, DisplayMode displayMode = DisplayMode.Hierglyph) {
        PickerGlyph newPickerGlyph = AddGlyph(glyphCode, displayMode);

        if (newPickerGlyph == null) return;

        _overlay.Activate();
        PlayerMechanics.Instance.SetControlMode(_instanceId, ControlMode.ControlsFrozen);
        newPickerGlyph.OnModeChanged += ModeChangeFinished;
        newPickerGlyph.Activate();

        _highlightImage.sprite = displayMode == DisplayMode.Literal ? newPickerGlyph.Glyph.Transliteration : newPickerGlyph.Glyph.Hieroglyph;

        if (PickerGlyphs.Count == 1) {
            _overlay.Activate();
            PlayerMechanics.Instance.SetControlMode(_instanceId, ControlMode.ControlsFrozen);
        }

        if (_newGlyph) {
            NewGlyphs.Enqueue(newPickerGlyph);
            foreach (PickerGlyph pickerGlyph in NewGlyphs) {
                pickerGlyph.Offset = _absoluteOffset;
            }
        }
        else {
            FocusOnGlyph(glyphCode);
        }

        _newGlyph = true;
        FocusChangedEventArgs.IsNewGlyph = _newGlyph;
    }

    private static PickerGlyph AddGlyph(string glyphCode, DisplayMode displayMode = DisplayMode.Literal) {
        if (PickerGlyphs.ContainsKey(glyphCode)) return null;

        ExtendedGlyph glyph = ResourceLoader.Get(glyphCode);
        if (glyph == null) return null;

        int slot = PickerGlyphs.Count;
        int glyphsBelow = PickerGlyphs.Count;

        foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values) {
            if (pickerGlyph.Glyph.AlphabeticIndex < glyph.AlphabeticIndex) {
                slot--;
                pickerGlyph.Move(1);
                glyphsBelow--;
            }
        }
        if (glyphsBelow == 0) {
            _absoluteOffset -= _rectTransform.rect.width / 2;
        }
        else if (_current.Slot > slot) {
            _absoluteOffset -= _rectTransform.rect.width / 2;
        }

        PickerGlyph newPickerGlyph = PickerGlyph.Create(glyph, slot, _transform, displayMode);

        PickerGlyphs.Add(glyphCode, newPickerGlyph);

        if (PickerGlyphs.Count == 1) {
            _current = newPickerGlyph;
            _last = newPickerGlyph;

            newPickerGlyph.Highlight();
            newPickerGlyph.Color = Global.Colors.HighlightYellow;

            _absoluteOffset += _rectTransform.rect.width / 2;

            instance.Activate();
        }

        foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values) {
            pickerGlyph.Offset = _absoluteOffset;
        }

        return newPickerGlyph;
    }

    private static void Clear() {
        foreach (PickerGlyph pickerGlyph in PickerGlyphs.Values) {
            pickerGlyph.Remove();
        }

        PickerGlyphs.Clear();
        _background.CrossFadeColor(BackgroundColor.Transparent(), 1, false, true);
        _absoluteOffset = 0;
    }

    public static void FocusOnGlyph(string glyphCode) {
        FocusOnGlyph(PickerGlyphs[glyphCode].Slot);
    }

    public static void FocusOnGlyph(int slot) {
        int currentSlot = _current.Slot;

        _toFocusOffset = _rectTransform.rect.width / 2 * (slot - currentSlot) + 0.001f;
        _animate = true;
    }

    private static void ModeChangeFinished(object sender, ModeChangedEventArgs e) {
        _highlightImage.sprite = e.AfterChange;
        _audioSource.pitch = 1;
        _audioSource.PlayOneShot(_addSound);
        _animatorActivatable.Activate();

        if (OnModeChanged != null) OnModeChanged(null, e);
    }

    private void AddAnimationFinished(object sender, EventArgs e) {
        if (NewGlyphs.Count > 0) {
            _newGlyph = true;
            FocusChangedEventArgs.IsNewGlyph = _newGlyph;

            PickerGlyph pickerGlyph = NewGlyphs.Dequeue();
            FocusOnGlyph(pickerGlyph.Slot);
        }
        else {
            _newGlyph = false;
            FocusChangedEventArgs.IsNewGlyph = _newGlyph;
        }

        if (_overlay == enabled) {
            _overlay.DeActivate();
            PlayerMechanics.Instance.UnSetControlMode(_instanceId, ControlMode.ControlsFrozen);
        }
    }

    private void FocusChanged(object sender, EventArgs e) {
        _audioSource.PlayOneShot(FocusChangeSound);
    }

    public static void MoveUp() {
        Move(0.375f);
    }

    public static void MoveDown() {
        Move(-0.375f);
    }

    private static void Move(float distance) {
        if (!_newGlyph) {
            _inputOffset += _rectTransform.rect.width * distance;
            _animate = true;
        }
    }

    private static void RemoveCurrent()
    {
        int slot = PickerGlyphs[_current.Glyph.GlyphCode].Slot;
        foreach (PickerGlyph p in PickerGlyphs.Values)
        {
            if (p.Slot > slot) p.Move(-1);
        }
        PickerGlyphs.Remove(_current.Glyph.GlyphCode);
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(LiteralPicker))]
    public class LiteralEditor : Editor {
        private static Texture2D _glyphCode;

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            _glyphCode = (Texture2D)EditorGUILayout.ObjectField("Glyph", _glyphCode, typeof(Texture2D), false);

            if (_glyphCode != null) {
                if (GUILayout.Button("Add Glyph")) {
                    AddGlyph(_glyphCode.name);
                }

                if (GUILayout.Button("Focus On Glyph")) {
                    FocusOnGlyph(_glyphCode.name);
                }
                GUILayout.Space(20);
            }

            LiteralPicker literalPicker = (LiteralPicker)target;

            if (GUILayout.Button("Activate")) {
                literalPicker.Activate();
            }
            if (GUILayout.Button("Deactivate")) {
                literalPicker.DeActivate();
            }

            if (GUILayout.Button("RemoveCurrent"))
            {
                RemoveCurrent();
            }

            if (GUILayout.Button("Clear")) {
                Clear();
            }
        }
    }
#endif
}

public class FocusChangedEventArgs : EventArgs {
    public bool IsNewGlyph { get; set; }
}