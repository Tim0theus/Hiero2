using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class ViperMechanic : Riddle, IPointerDownHandler, IPointerUpHandler {

    private Fader _fader;

    private void Start() {
        _fader = MaterialFader.Create(GetComponent<Renderer>(), new Color(0.2f, 0.2f, 0.2f), Color.black, true, 1, true);
        _fader.Activate();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            _fader.DeActivate();
            Solved();
        }
    }
}
