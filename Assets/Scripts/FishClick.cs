using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class FishClick : Riddle, IPointerDownHandler, IPointerUpHandler {

    private Fader _fader;

    private void Start()
    {
        _fader = MaterialFader.Create(GetComponentInChildren<Renderer>(), new Color(0.2f, 0.2f, 0.2f), Color.black, true, 1, true);
        _fader.Activate();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsSolved && eventData.button == PointerEventData.InputButton.Left)
        {
            _fader.DeActivate();
            SoundController.instance.Play("solved");
            Solved();
        }
    }
}
