using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hint : MonoBehaviour, IPointerClickHandler
{

    public static Hint instance;

    private float timerTime;

    private Image _image;

    private bool _isHidden;

    private Text _info;
    private Image _panel;
    private Button _button;

    public int mediumcosts;

    private HelperTrigger _current;
    public HelperTrigger GetCurrentHelper()
    {
        return _current;
    }

    public void OnPointerClick(PointerEventData d)
    {
        if (PlayerMechanics.Instance.CurrentDifficulty == DifficultyLevel.Easy || (PlayerMechanics.Instance.CurrentDifficulty == DifficultyLevel.Medium && GameControl.instance.GetPoints() > 9) || (PlayerMechanics.Instance.CurrentDifficulty == DifficultyLevel.Hard && GameControl.instance.GetPoints() > 19))
        {

                if (PlayerMechanics.Instance.CurrentDifficulty == DifficultyLevel.Medium) GameControl.instance.AddPoints(-mediumcosts++);
                if (PlayerMechanics.Instance.CurrentDifficulty == DifficultyLevel.Hard) GameControl.instance.AddPoints(-20);
                StartCoroutine(SwitchText());
                _info.CrossFadeAlpha(1, 0, true);
                _panel.CrossFadeAlpha(0.4f, 0, true);
                _button.targetGraphic.CrossFadeAlpha(1, 0, true);
                _button.targetGraphic.raycastTarget = true;
                _button.enabled = true;
                _image.CrossFadeAlpha(0, 0.1f, false);
                _image.raycastTarget = false;

        }
        else
        {
            SoundController.instance.Play("error");
        }
    }

    public IEnumerator SwitchText()
    {
        yield return new WaitForSeconds(15);
        _current.Use();
    }

    public void SetText(HelperTrigger h, string Info)
    {
        _current = h;
        if (Info == "")
        {
            _current = null;
        }
        if (Info != _info.text) timerTime = 15;
        _info.text = Info;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _image = GetComponent<Image>();
        _image.CrossFadeAlpha(0, 0, true);

        _info = GetComponentInChildren<Text>();

        _panel = GetComponentsInChildren<Image>()[1];
        _panel.CrossFadeAlpha(0, 0, true);

        _button = GetComponentInChildren<Button>();
        _button.targetGraphic.CrossFadeAlpha(0, 0, true);

        _isHidden = true;

        mediumcosts = 0;

        ResetTime();
    }

    private void Start()
    {
        Riddle[] list = GameObject.FindObjectsOfType<Riddle>();
        foreach (Riddle l in list)
        {
            l.onSolved += OnAction;
        }
    }

    public void OnAction(object o, EventArgs e)
    {
        DeActivate();
    }

    public void ResetTime()
    {
        timerTime = 15;
    }

    private void Update()
    {
        if (_info.text == "") DeActivate();
        else timerTime -= Time.deltaTime;

        if (timerTime <= 0)
            Activate();
    }

    public void Activate()
    {
        if (_isHidden && _image.sprite != null)
        {
            _image.CrossFadeAlpha(1, 0.1f, false);
            _isHidden = false;
            _image.raycastTarget = true;
        }
    }

    public void DeActivate()
    {
        ResetTime();
        if (_image.sprite != null)
        {
            _info.CrossFadeAlpha(0, 0, true);
            _panel.CrossFadeAlpha(0, 0, true);
            _button.targetGraphic.raycastTarget = false;
            _button.enabled = false;
            _button.targetGraphic.canvasRenderer.SetAlpha(0);
            _image.CrossFadeAlpha(0, 0.1f, false);
            _image.raycastTarget = false;
            _isHidden = true;
        }
    }
}
