using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour {
    public GameObject FirstToSelect;
    public ButtonControl BackButton;

    public bool open;

    private readonly List<UIControl> _uiElements = new List<UIControl>();

    private void Awake()
    {
        _uiElements.AddRange(transform.GetComponentsInChildren<UIControl>(false));
    }

    public void Start() { 
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

        open = true;
        enabled = true;
    }

    public void Close() {
        foreach (UIControl uiElement in _uiElements) {
            uiElement.DeActivate();
        }

        if (BackButton) {
            BackButton.DeActivate();
        }

        open = false;
        enabled = false;
    }

}
