using UnityEngine;

public class FullscreenJoystickControlScheme : ControlScheme {
    private void Awake() {
        MoveControl = GameObject.Find("JoystickLeft").GetComponent<VirtualJoystick>();
        LookControl = GameObject.Find("FullscreenJoystick").GetComponent<FullScreenJoystick>();
    }

    private void Update() {
        LookVector = LookControl.Vector;
        MoveVector = MoveControl.Vector;
    }

}

