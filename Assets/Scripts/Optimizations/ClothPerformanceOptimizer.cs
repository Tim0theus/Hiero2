using UnityEngine;

public class ClothPerformanceOptimizer : MonoBehaviour {
    private Cloth _cloth;

    private void Awake() {
        _cloth = GetComponent<Cloth>();
        _cloth.enabled = false;
    }

    private void OnBecameInvisible() {
        _cloth.enabled = false;
    
    }
    
    private void OnBecameVisible() {
        _cloth.enabled = true;
    }
}
