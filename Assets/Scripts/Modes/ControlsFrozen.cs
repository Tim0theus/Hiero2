using UnityEngine;

public class ControlsFrozen : Mode {

    public ControlsFrozen(GameObject canvas) {
        InputControlElement lookControlElement = PlayerControls.Instance.LookControlElement;
        InputControlElement moveControlElement = PlayerControls.Instance.MoveControlElement;

        Timer timer = canvas.GetComponentInChildren<Timer>();

        if (PlayerControls.Instance.CurrentType == ControlType.MouseKeyboard) {
            Activate.Add(PlayerControls.Instance.LookControlElement);
        }
        else {
            DeActivate.Add(lookControlElement);
        }

        DeActivate.Add(moveControlElement);
        DeActivate.Add(timer);
    }

}
