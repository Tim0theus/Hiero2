using System;
using UnityEngine;

public class Item : MonoBehaviour {
    public Transform Origin;

    private Vector3 _boundingRadius;
    private PickUp _pickup;
    private Renderer _renderer;

    private DynamicTwoTransformTransformer _transformer;


    private void Start() {
        _transformer = DynamicTwoTransformTransformer.Create(transform, 2);
        _pickup = transform.GetChild(0).GetComponent<PickUp>();
        _renderer = _pickup.GetComponent<Renderer>();

        _boundingRadius = _renderer.bounds.extents;
    }

    private void AddToInventory(object sender, EventArgs e) {
        Inventory.Add(_pickup.RiddleCode, this);
        Inventory.UnLock();
    }

    public void Pickup() {
        if (!_transformer.IsInTransition) {
            Inventory.Lock();
            transform.parent.position = _pickup.transform.position + _pickup.Offset * transform.parent.lossyScale.x;
            transform.parent.rotation = _pickup.transform.rotation;

            _pickup.transform.localPosition = -_pickup.Offset;
            _pickup.transform.localRotation = Quaternion.identity;

            _transformer.Set(Origin, Inventory.Slot, AddToInventory);
            _transformer.Activate();
        }
    }

    public void Putdown(Transform target) {
        if (!_transformer.IsInTransition) {
            _transformer.Set(Inventory.Slot, target);
            _transformer.Activate();
            _pickup.PutDown();
        }
    }

    private void ResetPickup(object sender, EventArgs e) {
        _pickup.Drop();
    }

    public void Drop(Vector3 worldPosition, Vector3 worldNormal) {
        if (!_transformer.IsInTransition) {
            Origin.position = worldPosition + new Vector3(worldNormal.x * _boundingRadius.x, worldNormal.y * _boundingRadius.y, worldNormal.z * _boundingRadius.z);
            Origin.rotation = Inventory.Slot.rotation;

            _transformer.Set(Inventory.Slot, Origin, ResetPickup);
            _transformer.Activate();
        }
    }
}