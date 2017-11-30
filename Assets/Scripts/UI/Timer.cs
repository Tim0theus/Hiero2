using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : UIControl, IEnableable {

    public EventHandler OnTimerElapsed;

    public float Minutes;

    public Image Indicator;
    public Image Background;

    private RectTransform _rectTransform;

    private Color _pulseColor;
    private Color _backgroundColor;

    private float _time;
    private float _minutes;

    private bool _enabled;

    private void Start() {
        OnTimerElapsed += PlayerMechanics.Instance.GameOver;

        _minutes = Minutes * 60;

        _rectTransform = Indicator.GetComponent<RectTransform>();
        Indicator.color = NormalColor;

        _pulseColor = NormalColor;
        _pulseColor.a = 0.25f;

        _backgroundColor = NormalColor;
        _backgroundColor.a = 0.5f;

        Disable(0);
        enabled = false;
    }

    private void Update() {
        _time += Time.deltaTime;

        float normalizedElapsedTime = (_minutes - _time) / _minutes;
        float smoothedElapsedTime = normalizedElapsedTime * normalizedElapsedTime;
        _rectTransform.localScale = new Vector3(smoothedElapsedTime, 1, 1);

        float pingPong = Mathf.PingPong(_time, 1);
        Indicator.color = Color.Lerp(NormalColor, _pulseColor, pingPong);

        if (smoothedElapsedTime <= 0) {
            enabled = false;

            if (OnTimerElapsed != null) {
                OnTimerElapsed(null, null);
            }
        }
    }

    public override void Activate() {
        if (_enabled)
            enabled = true;
    }

    public override void DeActivate() {
        enabled = false;
    }

    public void Enable() {
        Indicator.CrossFadeColor(NormalColor, FadeDuration, false, true);
        Background.CrossFadeColor(_backgroundColor, FadeDuration, false, true);

        _enabled = true;
    }

    public void Disable() {
        Disable(FadeDuration);

        _time = 0;
        _enabled = false;
        enabled = false;
    }

    private void Disable(float fadeDuration) {
        Indicator.CrossFadeColor(InactiveColor, fadeDuration, false, true);
        Background.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }
}

