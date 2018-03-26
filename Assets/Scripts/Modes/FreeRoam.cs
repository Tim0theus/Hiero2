using UnityEngine;

public class FreeRoam : Mode {

    public FreeRoam(GameObject canvas) {
        Translator translator = canvas.GetComponentInChildren<Translator>();

        Activate.Add(PlayerControls.Instance);
        Activate.Add(translator);
    }
}
