using System;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class WorldHighlightGlyph : Riddle, IActivatable, IPointerDownHandler, IPointerUpHandler {
    private const int DistanceMax = 5;

    [Range(0, 90)] public float ValidMaximumAngle;
    [Range(0, 1)] public float ValidAreaDiameter = 0.3f;
    [Range(0, DistanceMax)] public float MaximunDistanceToOrigin;

    public Transform LookFromOrigin;
    private float _alpha;
    private AudioSource _audioSource;

    private Transform _camera;
    private BoxCollider _collider;

    private Fader _fader;

    private Material _material;
    private float _time;

    private bool _touched;

    public void Activate() {
        _fader.Activate();
        _collider.enabled = true;
    }

    public void DeActivate() {
        _fader.DeActivate();
    }


    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (_alpha > 0.85) {
                Solved();
                _audioSource.Play();
                _fader.ActivateFormPoint(1);
                _touched = true;
            }
        }
    }


    private void Awake() {
        Renderer renderer = GetComponent<MeshRenderer>();

        _material = renderer.material;

        _fader = MaterialFader.Create(renderer, Color.black, true);
        _fader.AddPoint(Global.Colors.HighlightYellow);

        SetMaterialAlpha();

        _camera = Camera.main.transform;
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = false;

        _audioSource = GetComponent<AudioSource>();
    }

    private void SetMaterialAlpha() {
        Color color = _material.color;
        color.a = _alpha;
        _material.color = color;
    }


    private void Update() {
        if (!_touched) {
            float angle = (90 - ValidMaximumAngle) / 90;
            Vector3 originToGlyph = (transform.position - LookFromOrigin.position).normalized;
            _alpha = Vector3.Dot(originToGlyph, _camera.forward);
            _alpha = (_alpha - angle) / (1 - angle);
            _alpha = Mathf.Clamp01(_alpha);

            float distanceToOrigin = (LookFromOrigin.position - _camera.position).magnitude;
            _alpha *= (MaximunDistanceToOrigin - distanceToOrigin) / MaximunDistanceToOrigin * (ValidAreaDiameter * DistanceMax);
            _alpha = Mathf.Clamp01(_alpha);

            _alpha = _alpha * _alpha * _alpha;
        }
        else {
            float distanceToLookFromOrigin = (LookFromOrigin.position - _camera.position).magnitude;
            _alpha = (MaximunDistanceToOrigin - distanceToLookFromOrigin) / MaximunDistanceToOrigin;
            _alpha = Mathf.Clamp01(_alpha);
        }

        SetMaterialAlpha();

        if (!_touched && _alpha > 0.85) {
            _collider.enabled = true;
            _time += Time.deltaTime * 4;

            float scale = 1 - ((float)Math.Sin(_time - Math.PI / 2) / 20 + 1 / 20f);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else {
            _collider.enabled = false;

            float scale = transform.localScale.x;
            if (scale < 1) {
                transform.localScale += Vector3.one * scale / 500;
            }
            else {
                transform.localScale = Vector3.one;
                _time = 0;
            }
        }
    }

    public override void Enable() {
        Activate();
        base.Enable();
    }

    public override void Disable() {
        DeActivate();
        base.Disable();
    }
}