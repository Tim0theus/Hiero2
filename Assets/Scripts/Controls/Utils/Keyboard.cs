using UnityEngine;

public class Keyboard : InputControlElement {
    private Menu _mainMenu;
    private bool _unlocked;

    private void Start() {
        _mainMenu = GameObject.FindWithTag("MainMenu").GetComponent<Menu>();
    }

    private void Update() {
        if (_unlocked) {
            Vector.x = Input.GetAxis("Horizontal");
            Vector.y = Input.GetAxis("Vertical");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            PlayerMechanics.Instance.SetControlMode(ControlMode.DimScreen);
            _mainMenu.Open();
        }
    }

    public override void Activate() {
        _unlocked = true;
    }

    public override void DeActivate() {
        _unlocked = false;
    }
}