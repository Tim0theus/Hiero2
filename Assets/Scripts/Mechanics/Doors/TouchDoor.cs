using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AnimatorActivatable))]
public class TouchDoor : Riddle, IPointerDownHandler, IPointerUpHandler {

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left
            && eventData.pointerCurrentRaycast.distance < Global.Constants.TouchDistance) {
            Solved();
        }
    }
}
