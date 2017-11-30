using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioActivatable : Activatable {
    private AudioSource _audioSource;

    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        Riddle riddle = GetComponent<Riddle>();

        if (riddle) {
            riddle.Indicators.Add(this);
        }
    }

    public override void Activate() {
        _audioSource.Play();
    }

    public override void DeActivate() { }
}
