using System.Collections.Generic;
using UnityEngine;

public abstract class MenuState {
    protected readonly HashSet<GameObject> Activate = new HashSet<GameObject>();

    public void Enter() {
        foreach (GameObject activ in Activate) {
            activ.gameObject.SetActive(true);
        }
    }

    public void Leave() {
        foreach (GameObject activ in Activate) {
            activ.gameObject.SetActive(false);
        }
    }
}
