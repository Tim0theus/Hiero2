using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudio : MonoBehaviour {

    public List<AudioClip> FootStepsSounds;

    public bool moving = false;

    public string stepMaterial = "dirt";

    private AudioSource _audioSource;
    private int _index;

    private Vector3 _lastPosition;
    private Vector3 _parentlastPosition;
    private float _distance;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.material && hit.collider.material.name.Contains("Wood")) {
            stepMaterial = "wood";
        }
        else
        {
            stepMaterial = "dirt";
        }
    }

    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        _lastPosition = transform.position;
    }

    private void Update() {
        if (gameObject.transform.parent)
        {
            _distance += (transform.position - _lastPosition - transform.parent.position + _parentlastPosition).magnitude;
            _parentlastPosition = transform.parent.position;
        }
        else
        {
            _distance += (transform.position - _lastPosition).magnitude;
        }

        if (_distance > 0.8f && moving) {
            _distance = 0;

            if (stepMaterial == "dirt")
            _index = (Random.Range(0, 9)) % FootStepsSounds.Count;

            if (stepMaterial == "wood")
            _index = (Random.Range(11, 12)) % FootStepsSounds.Count;

            _audioSource.PlayOneShot(FootStepsSounds[_index]);
            moving = false;
        }

        _lastPosition = transform.position;
    }
}
