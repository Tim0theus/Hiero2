using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class RigidBodyPickUp : PickUp {
    private Rigidbody _rigidbody;
    private MeshCollider _meshCollider;

    private new void Awake() {
        base.Awake();

        _rigidbody = GetComponent<Rigidbody>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    protected override void Pickup() {
        base.Pickup();

        _rigidbody.isKinematic = true;
        _meshCollider.enabled = false;
    }

    public override void PutDown() {
        base.PutDown();

        _rigidbody.isKinematic = true;
        _meshCollider.enabled = true;
        enabled = false;
    }

    public override void Drop() {
        base.Drop();

        _rigidbody.isKinematic = false;
        _meshCollider.enabled = true;
    }

}
