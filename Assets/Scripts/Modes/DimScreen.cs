using UnityEngine;


public class DimScreen : Mode {

    public DimScreen(GameObject canvas) {
        InputControlElement lookControlElement = PlayerControls.Instance.LookControlElement;
        InputControlElement moveControlElement = PlayerControls.Instance.MoveControlElement;

        FullscreenOverlay fullscreenOverlay = canvas.transform.Find("BlackScreenUI").GetComponentInChildren<FullscreenOverlay>();
        Timer timer = canvas.GetComponentInChildren<Timer>();

        Activate.Add(fullscreenOverlay);

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
