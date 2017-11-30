using UnityEngine;

public class ControlsFrozen : Mode {

    public ControlsFrozen(PlayerControls playerControls, GameObject canvas) {
        InputControlElement lookControlElement = playerControls.LookControlElement;
        InputControlElement moveControlElement = playerControls.MoveControlElement;

        Timer timer = canvas.GetComponentInChildren<Timer>();

        if (playerControls.CurrentType == ControlType.MouseKeyboard) {
            Activate.Add(playerControls.LookControlElement);
        }
        else {
            DeActivate.Add(lookControlElement);
        }

        DeActivate.Add(moveControlElement);
        DeActivate.Add(timer);
    }

}
