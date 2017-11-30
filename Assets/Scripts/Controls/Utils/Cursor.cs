using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Cursor : FaderActivatable {
    public static Cursor Instance;

    private void Awake() {
        Instance = this;

        Fader = ImageFader.Create(GetComponent<Image>(), new Color(1, 1, 1, 0.5f), 3);
    }

    private void Update() {
        transform.position = Input.mousePosition + Vector3.forward * transform.position.z;
    }

    public void Hide() {
        enabled = false;
        Fader.DeActivate(ResetCursor);
    }

    private void ResetCursor(object sender, EventArgs e) {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        enabled = true;
    }

    public override void Activate() {
        base.Activate();
        enabled = true;
    }

    public override void DeActivate() {
        base.DeActivate();
        enabled = false;
    }
}