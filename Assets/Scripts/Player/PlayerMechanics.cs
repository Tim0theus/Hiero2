using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum ControlMode {
    FreeRoam,
    DimScreen,
    ControlsFrozen
}

public enum DifficultyLevel {
    Easy,
    Medium,
    Hard
}

public class PlayerMechanics : MonoBehaviour {

    public static PlayerMechanics Instance;

    private readonly Dictionary<ControlMode, Mode> _modes = new Dictionary<ControlMode, Mode>();

    [HideInInspector]
    public ControlMode CurrentControlMode;
    private static Mode _currentMode;
    private readonly List<ControlModeWithId> _modeList = new List<ControlModeWithId>();

    private readonly Dictionary<DifficultyLevel, Difficulty> _difficultyLevels = new Dictionary<DifficultyLevel, Difficulty>();

    [HideInInspector]
    public DifficultyLevel CurrentDifficulty;
    private static Difficulty _currentDifficulty;

    public EventHandler OnGameWon;
    public EventHandler OnGameOver;

    private static int _instanceId;

    private void Awake() {
        Instance = this;

        _instanceId = GetInstanceID();
    }
    private void Start() {

        _difficultyLevels.Add(DifficultyLevel.Easy, new EasyDifficulty());
        _difficultyLevels.Add(DifficultyLevel.Medium, new MediumDifficulty());
        _difficultyLevels.Add(DifficultyLevel.Hard, new HardDifficulty());

        foreach (Difficulty difficultyLevel in _difficultyLevels.Values) {
            difficultyLevel.Leave();
        }

        _currentDifficulty = _difficultyLevels[CurrentDifficulty];
        _currentDifficulty.Enter();

        PlayerControls playerControls = GetComponent<PlayerControls>();
        GameObject canvas = GameObject.FindWithTag("Canvas");

        _modes.Add(ControlMode.FreeRoam, new FreeRoam(playerControls, canvas));
        _modes.Add(ControlMode.DimScreen, new DimScreen(playerControls, canvas));
        _modes.Add(ControlMode.ControlsFrozen, new ControlsFrozen(playerControls, canvas));

        foreach (Mode mode in _modes.Values) {
            mode.Leave();
        }

#if !(UNITY_EDITOR)
        CurrentControlMode = ControlMode.FreeRoam;
#endif

        _currentMode = _modes[CurrentControlMode];
        _currentMode.Enter();

        _modeList.Add(new ControlModeWithId { Id = _instanceId, ControlMode = CurrentControlMode });

        OnGameOver += delegate { SetControlMode(_instanceId, ControlMode.ControlsFrozen); };
    }

    public void GameWon() {
        if (OnGameWon != null) OnGameWon(null, null);
    }

    public void GameOver(object sender = null, EventArgs e = null) {
        if (OnGameOver != null) OnGameOver(null, null);
    }

    public void SetDifficulty(int difficulty) {
        SetDifficulty((DifficultyLevel)difficulty);
    }
    private void SetDifficulty(DifficultyLevel difficulty) {
        if (difficulty == CurrentDifficulty) return;

        _currentDifficulty.Leave();
        _currentDifficulty = _difficultyLevels[difficulty];
        _currentDifficulty.Enter();

        CurrentDifficulty = difficulty;
    }


    public void SetControlMode(ControlMode controlMode) {
        SetControlMode(_instanceId, controlMode);
    }

    public void SetControlMode(int controlMode) {
        SetControlMode(_instanceId, (ControlMode)controlMode);
    }

    public void SetControlMode(int id, ControlMode controlMode) {
        if (controlMode == CurrentControlMode) return;

        SwichToMode(controlMode);
        _modeList.Add(new ControlModeWithId { Id = id, ControlMode = controlMode });
    }

    public void UnSetControlMode(int controlMode) {
        UnSetControlMode(_instanceId, (ControlMode)controlMode);
    }
    public void UnSetControlMode(int id, ControlMode controlMode) {
        if (_modeList.Count == 1) return;

        ControlModeWithId mostRecent = _modeList[_modeList.Count - 1];
        if (mostRecent.Id == id && mostRecent.ControlMode == controlMode) {
            _modeList.Remove(mostRecent);
            SwichToMode(_modeList[_modeList.Count - 1].ControlMode);
        }
        else {
            for (int i = _modeList.Count - 1; i >= 0; i--) {
                ControlModeWithId mode = _modeList[i];
                if (mode.Id == id && mode.ControlMode == controlMode) {
                    _modeList.Remove(mode);
                }
            }
        }

    }


    private void SwichToMode(ControlMode controlMode) {
        _currentMode.Leave();
        _currentMode = _modes[controlMode];
        _currentMode.Enter();

        CurrentControlMode = controlMode;
    }

    private struct ControlModeWithId {
        public int Id;
        public ControlMode ControlMode;
    }

#if UNITY_EDITOR
    private void Editor_SetControlMode(ControlMode controlMode) {
        if (Application.isPlaying) {
            SetControlMode(_instanceId, controlMode);
        }
        else {
            CurrentControlMode = controlMode;
        }
    }

    private void Editor_SetDifficulty(DifficultyLevel difficulty) {
        if (Application.isPlaying) {
            SetDifficulty(difficulty);
        }
        else {
            CurrentDifficulty = difficulty;
        }
    }

    [CustomEditor(typeof(PlayerMechanics))]
    public class PlayerMechanicsEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            PlayerMechanics playerMechanics = target as PlayerMechanics;
            ControlMode currentMode = playerMechanics.CurrentControlMode;
            ControlMode newMode = (ControlMode)EditorGUILayout.EnumPopup("Control Mode", currentMode);

            if (newMode != currentMode) {
                playerMechanics.Editor_SetControlMode(newMode);
            }

            DifficultyLevel currentDifficulty = playerMechanics.CurrentDifficulty;
            DifficultyLevel newDifficulty = (DifficultyLevel)EditorGUILayout.EnumPopup("Difficulty", currentDifficulty);

            if (newDifficulty != currentDifficulty) {
                playerMechanics.Editor_SetDifficulty(newDifficulty);
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Win")) {
                playerMechanics.GameWon();
            }

            if (GUILayout.Button("Lose")) {
                playerMechanics.GameOver();
            }

            GUILayout.EndHorizontal();
        }
    }
#endif
}


