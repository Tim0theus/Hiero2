using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : UIControl, IEnableable {

    public EventHandler OnTimerElapsed;

    public static Timer instance;

    public float Minutes;

    public Image Indicator;
    public Image Background;

    public Text timerText;

    private RectTransform _rectTransform;

    private Color _pulseColor;
    private Color _backgroundColor;

    private float _time;
    private float _minutes;

    private float _pointTimer;

    private bool _enabled;

    private Color NormalColor;

    private Coroutine _ticker;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        NormalColor = Indicator.color;
        _pointTimer = 0;

        Reset();
    }

    private void Start()
    {
        OnTimerElapsed += PlayerMechanics.Instance.GameOver;
    }

    public IEnumerator Ticker(float timeleft)
    {
        float time = 0;

        GetComponent<AudioSource>().Play();

        while (time < timeleft)
        {
            GetComponent<AudioSource>().pitch += 0.01f * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        GetComponent<AudioSource>().pitch = 1;
        GetComponent<AudioSource>().Stop();
    }

    private void Update() {
        _time += Time.deltaTime;
        _pointTimer += Time.deltaTime;

        timerText.text = "Time: " + (int)(_time / 60) + " minutes";

        if(_pointTimer > 10)
        {
            _pointTimer = 0;
            GameControl.instance.SubtractPoint(null, null);
        }

        float normalizedElapsedTime = (_minutes - _time);
        float smoothedElapsedTime = normalizedElapsedTime / _minutes * normalizedElapsedTime / _minutes;
        _rectTransform.localScale = new Vector3(smoothedElapsedTime, 1, 1);

        float pingPong = Mathf.PingPong(_time, 1);
        Indicator.color = Color.Lerp(Indicator.color, _pulseColor, pingPong);

        if (normalizedElapsedTime <= 60)
        {
            if (null == _ticker)
                _ticker = StartCoroutine(Ticker(_minutes - _time));
        }

        if (normalizedElapsedTime <= 0) {
            enabled = false;

            if (OnTimerElapsed != null) {
                OnTimerElapsed(null, null);
            }
        }
    }

    public float GetTime()
    {
        return _time;
    }

    public void SetTime(float time)
    {
        _time = time;
    }

    public void Reset()
    {
         _minutes = Minutes * 60;

        _rectTransform = Indicator.GetComponent<RectTransform>();
        Indicator.color = NormalColor;

        _pulseColor = NormalColor;
        _pulseColor.a = 0.25f;

        _backgroundColor = NormalColor;
        _backgroundColor.a = 0.5f;
        _time = 0;

        Disable(0);
        enabled = false;
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

