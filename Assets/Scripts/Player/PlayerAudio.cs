using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudio : MonoBehaviour {

    public List<AudioClip> FootStepsSounds;

    private AudioSource _audioSource;
    private int _index;

    private Vector3 _lastPosition;
    private float _distance;

    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        _lastPosition = transform.position;
    }

    private void Update() {
        _distance += (transform.position - _lastPosition).magnitude;

        if (_distance > 0.8f) {
            _distance = 0;

            _index = (_index + 17 + Random.Range(0, 5)) % FootStepsSounds.Count;

            _audioSource.PlayOneShot(FootStepsSounds[_index]);
        }

        _lastPosition = transform.position;
    }
}
