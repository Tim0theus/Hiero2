using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBox : MonoBehaviour, IActivatable, IPointerDownHandler, IPointerUpHandler {

    public static TextBox Instance { get; private set; }

    public static string Text {
        set {
            string text = value.Replace("\\n", "\n");
            Texts.Enqueue(text);
            SelfActivate();
        }
    }

    public static string EnqueueText {
        set {
            string text = value.Replace("\\n", "\n");
            Texts.Enqueue(text);
        }
    }

    private static readonly Queue<string> Texts = new Queue<string>();
    private static Text _textElement;
    private static Fader _backgroundFader;
    private static Fader _textFader;

    private static bool _displaying;
    private static bool _touched;

    private static int _instanceId;

    private void Start() {
        Instance = this;
        _instanceId = GetInstanceID();

        _textElement = GetComponentInChildren<Text>();

        _backgroundFader = ImageFader.Create(GetComponent<Image>(), Color.black / 2, (s, e) => { _touched = false; });
        _textFader = TextFader.Create(GetComponentInChildren<Text>(), Color.white);
    }

    private static void SelfActivate() {
        if (_displaying == false) {
            if (Texts.Count > 0) {
                _textElement.text = Texts.Dequeue();
            }

            _backgroundFader.Activate();
            _textFader.Activate();

            _displaying = true;
        }
    }

    private static void SelfDeactivate() {
        _backgroundFader.DeActivate(DisplayNext);
        _textFader.DeActivate();

    }

    private static void DisplayNext(object sender, EventArgs e) {
        _displaying = false;

        if (Texts.Count > 0) {
            SelfActivate();
        }
        else {
            PlayerMechanics.Instance.SetControlMode(_instanceId, ControlMode.FreeRoam);
        }
    }

    public void Activate() {
        SelfActivate();
    }

    public void DeActivate() {
        SelfDeactivate();
    }


    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (_touched == false) {
                SelfDeactivate();
                _touched = true;
            }
        }
    }
}

