using System.Collections.Generic;
using UnityEngine;

public class Difficulty : IMode {
    private readonly HashSet<IEnableable> _enable = new HashSet<IEnableable>();

    protected Difficulty(string difficultyTag) {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(difficultyTag);
        foreach (GameObject gameObject in gameObjects) {

            MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) {

                IEnableable enableable = script as IEnableable;

                if (enableable != null) {
                    _enable.Add(enableable);
                }
            }
        }
    }

    public virtual void Enter() {
        foreach (IEnableable element in _enable) {
            element.Enable();
        }
    }

    public virtual void Leave() {
        foreach (IEnableable element in _enable) {
            element.Disable();
        }
    }

}

