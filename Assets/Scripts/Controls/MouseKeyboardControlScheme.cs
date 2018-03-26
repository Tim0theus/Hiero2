public class MouseKeyboardControlScheme : ControlScheme {

    private void Awake() {
        LookControl = Mouse.Instance;
        MoveControl = gameObject.AddComponent<Keyboard>();
    }

    private void Update() {
        LookVector = LookControl.Vector;
        MoveVector = MoveControl.Vector;
    }

    public override void Activate() {
        Mouse.Instance.enabled = true;
        base.Activate();
    }

    public override void DeActivate() {
        Mouse.Instance.enabled = false;
        base.DeActivate();
    }
}