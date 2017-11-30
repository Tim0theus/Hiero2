using UnityEngine;

public class FreeRoam : Mode {

    public FreeRoam(PlayerControls playerControls, GameObject canvas) {
        Translator translator = canvas.GetComponentInChildren<Translator>();

        Activate.Add(playerControls);
        Activate.Add(translator);
    }
}
