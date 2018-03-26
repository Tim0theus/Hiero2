using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum ControlType {
    FullscreenAndTouchJoystick,
    TouchJoysticksOnly,
    MouseKeyboard
}

public class PlayerControls : MonoBehaviour, IActivatable {

    public static PlayerControls Instance;

    public InputControlElement LookControlElement { get { return _currentScheme.LookControl; } }
    public InputControlElement MoveControlElement { get { return _currentScheme.MoveControl; } }

    [HideInInspector]
    public float MoveMultiplier = 1f;

    private const float DefaultMoveSpeed = 1.25f;
    private const float SprintMultiplier = 1.25f;

    private const float LookMultiplier = 90;
    private const float LookAngle = 50;

    private float _userLookMultiplier = 1;
    private int _invertLookY = 1;

    private float _pitch;
    private float _yaw;

    private CharacterController _characterController;
    private Camera _camera;

    [HideInInspector] public ControlType CurrentType;
    private ControlScheme _currentScheme;

    private Dictionary<ControlType, ControlScheme> _controls;

    public void Activate() {
        _currentScheme.Activate();
    }

    public void DeActivate() {
        _currentScheme.DeActivate();
    }

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _controls = new Dictionary<ControlType, ControlScheme>();

        _controls.Add(ControlType.FullscreenAndTouchJoystick, gameObject.AddComponent<FullscreenJoystickControlScheme>());
        _controls.Add(ControlType.TouchJoysticksOnly, gameObject.AddComponent<TouchJoystickControlScheme>());
        _controls.Add(ControlType.MouseKeyboard, gameObject.AddComponent<MouseKeyboardControlScheme>());

        foreach (IActivatable control in _controls.Values) {
            control.DeActivate();
        }

        switch (Application.platform) {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                CurrentType = ControlType.FullscreenAndTouchJoystick;
                break;
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WebGLPlayer:
                CurrentType = ControlType.MouseKeyboard;
                break;
        }


#if UNITY_EDITOR
        UnityEngine.Cursor.visible = true;
#else
        Transform spawn = GameObject.FindGameObjectWithTag("Spawn").transform;
        transform.position = spawn.position;
#endif
        _currentScheme = _controls[CurrentType];

    }

    private void Update() {
        float clampedDeltaTime = Math.Min(0.1f, Time.deltaTime);

        //Looking
        _pitch += -1 * _currentScheme.LookVector.y * LookMultiplier * _userLookMultiplier * _invertLookY * clampedDeltaTime;
        _yaw += _currentScheme.LookVector.x * LookMultiplier * _userLookMultiplier * clampedDeltaTime;

        _pitch = Mathf.Clamp(_pitch, -LookAngle, LookAngle);

        transform.eulerAngles = new Vector3(0, _yaw, 0);
        _camera.transform.eulerAngles = new Vector3(_pitch, _yaw, 0);

        //Walking
        float sprint = Input.GetAxis("Sprint");

        float movementInLookDir = _currentScheme.MoveVector.y * DefaultMoveSpeed * MoveMultiplier;
        float movementSideways = _currentScheme.MoveVector.x * DefaultMoveSpeed * MoveMultiplier;

        //Add sprint
        movementInLookDir += movementInLookDir * sprint * SprintMultiplier;
        movementSideways += movementSideways * sprint * SprintMultiplier;

        Vector3 transformedLookDir = transform.forward * movementInLookDir;
        Vector3 transformedSideways = transform.right * movementSideways;
        Vector3 gravity = Physics.gravity;

        if (_currentScheme.MoveVector.magnitude != 0) GetComponent<PlayerAudio>().moving = true;

        _characterController.Move((transformedLookDir + transformedSideways + gravity) * Time.deltaTime);
    }

    public void SetMoveSpeed(float value)
    {
        MoveMultiplier = Math.Max(1.0f, value);
    }

    public void SetUserLookMultiplier(float value) {
        _userLookMultiplier = value;
    }

    public void SetInvertLookY(bool value) {
        if (value) { _invertLookY = -1; }
        else { _invertLookY = 1; }
    }
    public void SetControlScheme(int value) {
        SetControlScheme((ControlType)value);
    }

    private void SetControlScheme(ControlType controlType) {
        if (controlType == CurrentType) return;

        _currentScheme.DeActivate();
        _currentScheme = _controls[controlType];
        _currentScheme.Activate();

        CurrentType = controlType;
    }


#if UNITY_EDITOR
    private void Editor_SetControlScheme(ControlType controlType) {
        if (Application.isPlaying) {
            SetControlScheme(controlType);
        }
        else {
            CurrentType = controlType;
        }
    }

    [CustomEditor(typeof(PlayerControls))]
    public class PlayerControlsEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            PlayerControls playerControls = (PlayerControls)target;
            ControlType current = playerControls.CurrentType;
            ControlType newControlType = (ControlType)EditorGUILayout.EnumPopup("Control Type", current);

            if (newControlType != current) {
                playerControls.Editor_SetControlScheme(newControlType);
            }
        }
    }






    [CustomEditor(typeof(CharacterController))]
    public class CharacterControllerEditor : Editor {

        public static Transform SceneCamera;

        public void OnSceneGUI() {
            SceneCamera = SceneView.currentDrawingSceneView.camera.transform;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            GUILayout.Label("Play from");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("HERE")) {
                CharacterController characterController = (CharacterController)target;

                RaycastHit hit;
                Ray ray = new Ray(SceneCamera.position, Vector3.down);
                Physics.Raycast(ray, out hit);

                if (hit.point.sqrMagnitude > 0.0001f && Math.Abs(hit.point.y) < 5) {
                    characterController.transform.position = new Vector3(hit.point.x, hit.point.y + characterController.height, hit.point.z);
                    EditorApplication.isPlaying = true;
                }

            }

            if (GUILayout.Button("SPAWN")) {
                CharacterController characterController = (CharacterController)target;

                Transform spawn = GameObject.FindGameObjectWithTag("Spawn").transform;
                characterController.transform.position = spawn.position;

                EditorApplication.isPlaying = true;
            }

            GUILayout.EndHorizontal();
        }
    }

#endif
}
