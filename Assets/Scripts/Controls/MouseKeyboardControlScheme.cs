public class MouseKeyboardControlScheme : ControlScheme {
    private Mouse _mouse;

    private void Awake() {
        _mouse = Mouse.Instance;

        LookControl = _mouse;
        MoveControl = gameObject.AddComponent<Keyboard>();
    }

    private void Update() {
        LookVector = LookControl.Vector;
        MoveVector = MoveControl.Vector;
    }

    public override void Activate() {
        _mouse.enabled = true;
        base.Activate();
    }

    public override void DeActivate() {
        _mouse.enabled = false;
        base.DeActivate();
    }
}