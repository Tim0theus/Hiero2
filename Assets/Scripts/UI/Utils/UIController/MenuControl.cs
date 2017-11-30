using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum MenuType {
    Touch,
    MouseKeyboard
}

public class MenuControl : MonoBehaviour {
    [HideInInspector] public MenuType CurrentType;
    private MenuState _currentState;

    private readonly Dictionary<MenuType, MenuState> _menuStates = new Dictionary<MenuType, MenuState>();

    private void Start() {
        _menuStates.Add(MenuType.Touch, new TouchMenu());
        _menuStates.Add(MenuType.MouseKeyboard, new KeyboardMouseMenu());

        foreach (MenuState menuState in _menuStates.Values) {
            menuState.Leave();
        }

        switch (PlayerControls.Instance.CurrentType) {
            case ControlType.FullscreenAndTouchJoystick:
            case ControlType.TouchJoysticksOnly:
                CurrentType = MenuType.Touch;
                break;
            case ControlType.MouseKeyboard:
                CurrentType = MenuType.MouseKeyboard;
                break;
        }

        _currentState = _menuStates[CurrentType];
        _currentState.Enter();
    }

    private void SetMenuState(MenuType menuType) {
        if (menuType == CurrentType) return;

        _currentState.Enter();
        _currentState = _menuStates[menuType];
        _currentState.Leave();

        CurrentType = menuType;
    }

#if UNITY_EDITOR
    private void Editor_SetControlScheme(MenuType controlType) {
        if (Application.isPlaying) {
            SetMenuState(controlType);
        }
        else {
            CurrentType = controlType;
        }
    }

    [CustomEditor(typeof(MenuControl))]
    public class MenuControlEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            MenuControl menuControl = (MenuControl)target;
            MenuType current = menuControl.CurrentType;
            MenuType newControlType = (MenuType)EditorGUILayout.EnumPopup("Menu Type", current);

            if (newControlType != current) {
                menuControl.Editor_SetControlScheme(newControlType);
            }
        }
    }
#endif
}
