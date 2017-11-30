using UnityEngine;

public class TouchMenu : MenuState {
    public TouchMenu() {
        GameObject[] touchElements = GameObject.FindGameObjectsWithTag("TouchElement");

        foreach (GameObject touchElement in touchElements) {
            Activate.Add(touchElement);
        }
    }
}
