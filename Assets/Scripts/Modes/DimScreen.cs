using UnityEngine;


public class DimScreen : Mode {

    public DimScreen(PlayerControls playerControls, GameObject canvas) {
        InputControlElement lookControlElement = playerControls.LookControlElement;
        InputControlElement moveControlElement = playerControls.MoveControlElement;

        FullscreenOverlay fullscreenOverlay = canvas.transform.Find("BlackScreenUI").GetComponentInChildren<FullscreenOverlay>();
        Timer timer = canvas.GetComponentInChildren<Timer>();

        Activate.Add(fullscreenOverlay);

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
