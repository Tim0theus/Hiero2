using UnityEngine;
using UnityEngine.UI;

public class GameWon : MonoBehaviour {

    public Text _text;

    private void OnTriggerEnter(Collider other) {
        PlayerMechanics.Instance.GameWon();
        SoundController.instance.Play("win");
        PlayerMechanics.Instance.GetComponent<AudioSource>().Stop();
        _text.text = "Points: " + GameControl.instance.GetPoints();
    }
}
