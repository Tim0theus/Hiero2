using UnityEngine;

public class InvisibleWall : MonoBehaviour {
    private void Start() {
        GetComponent<MeshRenderer>().enabled = false;
    }

}
