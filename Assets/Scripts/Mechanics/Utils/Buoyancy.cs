using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class Buoyancy : MonoBehaviour {
    [Range(0, 100)] public float SurfaceOffset;
    public float UpwardForce;

    private float _upperLimit;
    private float _volumeHeight;

    private void Start() {
        _upperLimit = GetComponent<Collider>().bounds.max.y;
        _volumeHeight = GetComponent<Collider>().bounds.size.y;
    }

    private void OnTriggerEnter(Collider other) {
        other.attachedRigidbody.drag = 4;
    }

    private void OnTriggerStay(Collider other) {
        float lowerLimit = other.bounds.min.y;
        float distanceToSurface = Math.Abs(_upperLimit - lowerLimit);

        if (distanceToSurface > SurfaceOffset / 100 * _volumeHeight) {
            float force = Random.value + Math.Abs(Physics.gravity.y) + UpwardForce;

            other.attachedRigidbody.AddRelativeForce(0, force, 0, ForceMode.Acceleration);
        }
    }
}