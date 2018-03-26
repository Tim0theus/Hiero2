using UnityEngine;
using UnityEngine.UI;

public class Mouse : InputControlElement {
    public static Mouse Instance;

    private Cursor _cursor;
    private Fader _fader;

    private bool _cameraActive;
    private bool _lockCamera;

    private bool _invertScroll;
    private float _scroll;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UnityEngine.Cursor.visible = false;

        _cursor = Cursor.Instance;
        _fader = ImageFader.Create(GetComponent<Image>(), new Color(1, 1, 1, 0.3f), 3);
    }

    private void Update() {
        if (_cameraActive) {
            if (_lockCamera == false && Input.GetMouseButtonDown(0)) LockCamera();
            else if (_lockCamera && Input.GetMouseButtonDown(1)) UnlockCamera();

            if (_lockCamera == false) {
                Vector.x = Input.GetAxis("Mouse X");
                Vector.y = Input.GetAxis("Mouse Y");
            }
            else {
                Vector = Vector2.zero;
            }

            _scroll = Input.mouseScrollDelta.y;

            if (_scroll < -0.1) {
                if (_invertScroll) LiteralPicker.MoveDown();
                else LiteralPicker.MoveUp();
                _scroll = 0;
            }
            else if (_scroll > 0.1) {
                if (_invertScroll) LiteralPicker.MoveUp();
                else LiteralPicker.MoveDown();
                _scroll = 0;
            }
        }
    }

    private void LockCamera() {
        _lockCamera = true;
        _cursor.enabled = true;
        _fader.DeActivate();
        _cursor.Activate();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    private void UnlockCamera() {
        _fader.Activate();
        _cursor.Hide();
        _lockCamera = false;
    }

    public void SetInvertScroll(bool value) {
        _invertScroll = value;
    }

    public override void Activate() {
        if (enabled) {
            _cameraActive = true;
            UnlockCamera();
        }
        else {
            _cursor.Activate();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

    public override void DeActivate() {
        if (enabled) {
            _cameraActive = false;
            LockCamera();
        }
        else {
            _cursor.DeActivate();
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }
}