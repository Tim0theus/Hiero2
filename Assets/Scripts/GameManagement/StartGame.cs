using UnityEngine;

public class StartGame : MonoBehaviour {
    private FullscreenOverlay _mainOverlay;

    private void Awake() {
        _mainOverlay = GameObject.FindGameObjectWithTag("MainOverlay").GetComponent<FullscreenOverlay>();
    }

    private void Start() {
        _mainOverlay.DeActivate();
    }
}

