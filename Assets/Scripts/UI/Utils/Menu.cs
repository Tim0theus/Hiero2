using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour {
    public GameObject FirstToSelect;
    public ButtonControl BackButton;
    public bool NoGroupControlInChildren;

    private readonly List<UIControl> _uiElements = new List<UIControl>();

    private void Start() {
        if (NoGroupControlInChildren) {
            _uiElements.AddRange(transform.GetComponentsInChildren<UIControl>(false));
        }
        else {
            _uiElements.AddRange(transform.GetComponentsInChildren<GroupControl>(false));
            _uiElements.AddRange(transform.GetComponentsInChildren<ButtonControl>(false));
        }

        Close();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (BackButton) {
                BackButton.Click();
            }
        }
    }

    public void Open() {
        if (FirstToSelect) EventSystem.current.SetSelectedGameObject(FirstToSelect);

        foreach (UIControl uiElement in _uiElements) {
            uiElement.Activate();
        }

        if (BackButton) {
            BackButton.Activate();
        }

        enabled = true;
    }

    public void Close() {
        foreach (UIControl uiElement in _uiElements) {
            uiElement.DeActivate();
        }

        if (BackButton) {
            BackButton.DeActivate();
        }

        enabled = false;
    }

}
