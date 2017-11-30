using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour, IActivatable {
    private IFadeable _fadeable;
    private readonly List<FaderPoint> _points = new List<FaderPoint>();
    private FaderPoint _from;
    private FaderPoint _to;

    private int _transitionCursor;
    private Color _currentColor;

    private int _direction = 1;
    private float _transition;

    private int _blinkProgressCount;
    private bool _blinkInfinite;

    private void Awake() {
        enabled = false;
    }

    private void Start() {
        _fadeable.Color = _currentColor = _points[0].Color;
    }

    private void Update() {
        if (0 <= _transition && _transition <= 1) {
            _transition += Time.deltaTime * _direction * _to.Speed;
            _currentColor = Color.Lerp(_from.Color, _to.Color, _transition);
            _fadeable.Color = _currentColor;
        }
        else {
            _transition = Mathf.Clamp01(_transition);

            if (_blinkInfinite && _to.Flag != Flag.EndPoint) {
                _direction *= -1;
            }
            else if (_blinkProgressCount < _to.BlinkCount) {
                _direction *= -1;
                _blinkProgressCount++;
            }
            else {
                enabled = false;

                _blinkProgressCount = 0;

                if (_to.Flag == Flag.EndPoint) {

                    _currentColor = _to.Color;

                    if (_to.Color.a < 0.01f) {
                        _fadeable.Hide();
                    }
                    else {
                        _fadeable.Disable();
                    }
                }

                _to.TransitionFinished();

                if (_transitionCursor + 1 < _points.Count) {
                    _transitionCursor++;
                    _from = _to;
                    _to = _points[_transitionCursor];
                }

            }
        }

    }

    public void AddPoint(Color color, float speed = 1, int blinkCount = 0) {
        AddPoint(color, null, speed, blinkCount);
    }

    public void AddPoint(Color color, EventHandler eventHandler, float speed = 1, int blinkCount = 0) {
        _points.Add(new FaderPoint(color, speed, blinkCount, eventHandler));
    }

    public void Activate() {
        _direction = 1;
        _transition = 0;
        _blinkProgressCount = 0;
        _from = _points[0];
        _to = _points[1];
        _fadeable.Show();
        enabled = true;
    }

    public void ActivateFormPoint(int index, bool blinkInfinite = false) {
        _direction = 1;
        _transition = 0;
        _blinkProgressCount = 0;
        _from = _points[index];
        _to = _points[index + 1];
        _fadeable.Show();
        _blinkInfinite = blinkInfinite;
        enabled = true;
    }

    public void DeActivate() {
        _direction = 1;
        _transition = 0;
        _blinkProgressCount = 0;
        _from = new FaderPoint(_currentColor);
        _to = new FaderPoint(_points[0].Color, Flag.EndPoint, _points[0].Speed);
        _fadeable.Show();
        enabled = true;
    }

    public void DeActivate(EventHandler eventHandler) {
        DeActivate();
        _to = new FaderPoint(_points[0].Color, Flag.EndPoint, _points[0].Speed, 0, eventHandler);
    }

    public static Fader Create(IFadeable fadeable, Color color, Color inactiveColor, EventHandler eventHandler, float speed, bool blinkInfinte, int blinkCount) {
        Fader fader = fadeable.GameObject.AddComponent<Fader>();
        fader._fadeable = fadeable;

        fader._fadeable.Color = inactiveColor;

        fader.AddPoint(inactiveColor, speed);
        fader.AddPoint(color, eventHandler, speed, blinkCount);

        fader._blinkInfinite = blinkInfinte;

        return fader;
    }
}

public static class ImageFader {
    public static Fader Create(Image image, Color color, float speed = 1, int blinkCount = 0) {
        return Create(image, color, color.Transparent(), null, speed, blinkCount);
    }

    public static Fader Create(Image image, Color color, EventHandler eventHandler, float speed = 1, int blinkCount = 0) {
        return Create(image, color, color.Transparent(), eventHandler, speed, blinkCount);
    }

    public static Fader Create(Image image, Color color, Color inactiveColor, float speed = 1, int blinkCount = 0) {
        return Create(image, color, inactiveColor, null, speed, blinkCount);
    }

    private static Fader Create(Image image, Color color, Color inactiveColor, EventHandler eventHandler, float speed, int blinkCount) {
        ImageFadeable fadeable = new ImageFadeable(image);
        return Fader.Create(fadeable, color, inactiveColor, eventHandler, speed, false, blinkCount);
    }
}
public static class MaterialFader {
    public static Fader Create(Renderer renderer, Color color, float speed = 1, int blinkCount = 0) {
        return Create(renderer, color, color.Transparent(), false, null, speed, false, blinkCount);
    }
    public static Fader Create(Renderer renderer, Color color, bool emissive, float speed = 1, int blinkCount = 0) {
        return Create(renderer, color, color.Transparent(), emissive, null, speed, false, blinkCount);
    }
    public static Fader Create(Renderer renderer, Color color, EventHandler eventHandler, float speed = 1, int blinkCount = 0) {
        return Create(renderer, color, color.Transparent(), false, eventHandler, speed, false, blinkCount);
    }
    public static Fader Create(Renderer renderer, Color color, Color inactiveColor, float speed = 1, int blinkCount = 0) {
        return Create(renderer, color, inactiveColor, false, null, speed, false, blinkCount);
    }
    public static Fader Create(Renderer renderer, Color color, Color inactiveColor, bool emissive, float speed = 1, int blinkCount = 0) {
        return Create(renderer, color, inactiveColor, emissive, null, speed, false, blinkCount);
    }
    public static Fader Create(Renderer renderer, Color color, Color inactiveColor, bool emisive, float speed, bool blinkInfinite) {
        return Create(renderer, color, inactiveColor, emisive, null, speed, blinkInfinite, 0);
    }
    public static Fader Create(Renderer renderer, Color color, bool emissive, EventHandler eventHandler, float speed = 1, int blinkCount = 0) {
        return Create(renderer, color, color.Transparent(), emissive, eventHandler, speed, false, blinkCount);
    }
    public static Fader Create(Renderer renderer, Color color, bool emissive, bool blinkInfinite, float speed = 1) {
        return Create(renderer, color, color.Transparent(), emissive, null, speed, blinkInfinite, 0);
    }

    private static Fader Create(Renderer renderer, Color color, Color inactiveColor, bool emissive, EventHandler eventHandler, float speed, bool blinkInfinite, int blinkCount) {
        if (emissive) {
            EmissiveMaterialFadeable fadeable = new EmissiveMaterialFadeable(renderer);
            return Fader.Create(fadeable, color, inactiveColor, eventHandler, speed, blinkInfinite, blinkCount);
        }
        else {
            MaterialFadeable fadeable = new MaterialFadeable(renderer);
            return Fader.Create(fadeable, color, inactiveColor, eventHandler, speed, blinkInfinite, blinkCount);
        }
    }
}
public static class TextFader {
    public static Fader Create(Text text, Color color, float speed = 1, int blinkCount = 0) {
        return Create(text, color, color.Transparent(), null, speed, blinkCount);
    }
    private static Fader Create(Text text, Color color, Color inactiveColor, EventHandler eventHandler, float speed, int blinkCount) {
        TextFadeable fadeable = new TextFadeable(text);
        return Fader.Create(fadeable, color, inactiveColor, eventHandler, speed, false, blinkCount);
    }
}

public class FaderPoint {

    public Flag Flag { get; private set; }
    public Color Color { get; private set; }
    public float Speed { get; private set; }
    public int BlinkCount { get; private set; }
    private event EventHandler OnTransitionFinished;

    public FaderPoint(Color color, float speed = 1, int blinkCount = 0, EventHandler transitionFinished = null) {
        Color = color;
        Speed = speed;
        BlinkCount = Math.Max(0, blinkCount * 2 - 1);
        OnTransitionFinished = transitionFinished;
    }

    public FaderPoint(Color color, Flag flag, float speed = 1, int blinkCount = 0, EventHandler transitionFinshed = null) : this(color, speed, blinkCount, transitionFinshed) {
        Flag = flag;
    }

    public void TransitionFinished() {
        if (OnTransitionFinished != null) OnTransitionFinished(null, null);
    }
}

public enum Flag {
    RegularPoint,
    StartPoint,
    EndPoint
}
