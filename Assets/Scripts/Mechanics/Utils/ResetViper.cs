using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResetViper : MonoBehaviour {

    public float MaxTravelDistance = 10;

    private Animator _animator;
    private float _timer;
    private const float TimeoutScale = 3;
    private Vector3 _startPosition;

    private void Start() {
        _animator = GetComponent<Animator>();
        _startPosition = transform.position;

        _timer = Random.value * TimeoutScale;
    }

    private void Update() {

        if (_timer > 0) {
            _timer -= Time.deltaTime;
        }
        else if (Math.Abs(transform.localPosition.x) > MaxTravelDistance) {
            _animator.enabled = false;
            transform.position = _startPosition;
            _timer = Random.value * TimeoutScale;
        }

        if (_timer < 0) {
            _animator.enabled = true;
        }
    }

}
