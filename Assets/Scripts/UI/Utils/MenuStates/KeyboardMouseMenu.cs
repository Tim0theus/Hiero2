using UnityEngine;

public class KeyboardMouseMenu : MenuState {
    public KeyboardMouseMenu() {
        GameObject[] mouseKeyboardElements = GameObject.FindGameObjectsWithTag("MouseKeyboardElement");

        foreach (GameObject mouseKeyboardElement in mouseKeyboardElements) {
            Activate.Add(mouseKeyboardElement);
        }
    }
}
