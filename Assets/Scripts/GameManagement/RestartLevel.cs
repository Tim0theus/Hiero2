using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour {
    private FullscreenOverlay _mainOverlay;
    private int _currentSceneIndex;

    private void Awake() {
        _mainOverlay = GameObject.FindGameObjectWithTag("MainOverlay").GetComponent<FullscreenOverlay>();
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void Restart() {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel() {
        _mainOverlay.Activate();
        yield return new WaitForSeconds(_mainOverlay.FadeDuration);

        SceneManager.LoadScene(_currentSceneIndex);
    }
}
