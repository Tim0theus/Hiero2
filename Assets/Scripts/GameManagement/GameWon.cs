using UnityEngine;

public class GameWon : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        PlayerMechanics.Instance.GameWon();
    }
}
