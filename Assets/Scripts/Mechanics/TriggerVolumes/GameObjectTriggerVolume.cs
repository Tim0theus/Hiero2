using System.Collections.Generic;
using UnityEngine;

public class GameObjectTriggerVolume : TriggerVolume {
    public List<GameObject> GameObjects;

    protected override void Activate() {
        foreach (GameObject activatable in GameObjects) {
            activatable.SetActive(true);
        }
    }

    protected override void Check()
    {
    }

    protected override void Deactivate() {
        foreach (GameObject activatable in GameObjects) {
            activatable.SetActive(false);
        }
    }
}
