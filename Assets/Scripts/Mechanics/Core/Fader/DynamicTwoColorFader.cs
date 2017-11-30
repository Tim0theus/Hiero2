using System;
using UnityEngine;
using UnityEngine.UI;

public class DynamicTwoColorFader : Activatable {
    public Color DeactiveColor { private get; set; }
    public Color ActiveColor { private get; set; }

    private IFadeable _fadeable;

    private int _direction = 1;
    private float _speed;
    private float _transition;

    public event EventHandler Activated;
    public event EventHandler DeActivated;

    public static DynamicTwoColorFader Create(Image image, float speed = 1) {
        DynamicTwoColorFader fader = image.gameObject.AddComponent<DynamicTwoColorFader>();
        ImageFadeable fadeable = new ImageFadeable(image);
        fader._fadeable = fadeable;
        fader._speed = speed;

        return fader;
    }

    private void Start() {
        _fadeable.Color = DeactiveColor;
    }

    private void Update() {
        if (0 <= _transition && _transition <= 1) {
            _transition += Time.deltaTime * _direction * _speed;
            _fadeable.Color = Color.Lerp(DeactiveColor, ActiveColor, _transition);
        }
        else {
            enabled = false;

            _transition = Mathf.Clamp01(_transition);

            if (_transition >= 1) {
                if (Activated != null) Activated(null, null);
                Activated = null;
            }
            else {
                if (DeActivated != null) DeActivated(null, null);
                DeActivated = null;
            }
        }
    }

    public override void Activate() {
        _direction = 1;
        enabled = true;
    }

    public override void DeActivate() {
        _direction = -1;
        enabled = true;
    }

    public void Activate(EventHandler eventHandler) {
        Activated += eventHandler;
        Activate();
    }

    public void DeActivate(EventHandler eventHandler) {
        DeActivated += eventHandler;
        DeActivate();
    }
}