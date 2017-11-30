using UnityEngine;

public class TouchJoystickControlScheme : ControlScheme {
    private const float Damping = 0.6f;

    private void Awake() {
        MoveControl = GameObject.Find("JoystickLeft").GetComponent<VirtualJoystick>();
        LookControl = GameObject.Find("JoystickRight").GetComponent<VirtualJoystick>();
    }

    private void Update() {
        float x = LookControl.Vector.x;
        float y = LookControl.Vector.y;

        LookVector.x = (x * x) * Mathf.Sign(x) * Damping;
        LookVector.y = (y * y) * Mathf.Sign(y) * Damping;

        MoveVector = MoveControl.Vector;
    }

}

