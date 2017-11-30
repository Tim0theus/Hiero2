using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FloatingHighlight : AnimatorActivatable {
    public void Start() {
        GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
    }

    public override void Activate() {
        Vector3 toCamera = (Camera.main.transform.position - transform.position).normalized;
        transform.forward = new Vector3(-toCamera.x, 0, -toCamera.z);

        base.Activate();
    }
}
