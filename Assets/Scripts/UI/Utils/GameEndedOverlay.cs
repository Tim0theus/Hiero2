using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEndedOverlay : MonoBehaviour {
    public bool OnGameWon;
    public bool OnGameOver;

    private readonly List<UIControl> _uiElements = new List<UIControl>();

    protected void Start() {
        _uiElements.AddRange(transform.GetComponentsInChildren<UIControl>(false));

        foreach(UIControl uiElement in _uiElements) {
            uiElement.DeActivate();
        }

        if(OnGameWon) {
            PlayerMechanics.Instance.OnGameWon += Open;
        }
        if(OnGameOver) {
            PlayerMechanics.Instance.OnGameOver += Open;
        }
    }

    private void Open(object sender, EventArgs e) {
        foreach(UIControl uiElement in _uiElements) {
            uiElement.Activate();
        }
    }
}