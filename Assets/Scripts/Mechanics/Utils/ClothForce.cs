using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClothForce : Riddle, IPointerDownHandler, IPointerUpHandler {
    private Cloth _cloth;
    private Transform _clothColliderTransform;

    private ClothSphereColliderPair[] _clothSphereColliders;
    private ClothSphereColliderPair[] _defaultSphereColliders;

    public void OnPointerDown(PointerEventData eventData) {
        if(eventData.button == PointerEventData.InputButton.Left) {
            RaycastResult currentRaycast = eventData.pointerCurrentRaycast;

            _clothColliderTransform.position = currentRaycast.worldPosition + currentRaycast.worldNormal * 0.05f;
            _cloth.sphereColliders = _clothSphereColliders;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        RaycastResult currentRaycast = eventData.pointerCurrentRaycast;
        Vector3 localPosition = _clothColliderTransform.localPosition;

        _cloth.sphereColliders = _defaultSphereColliders;
        _clothColliderTransform.position = currentRaycast.worldPosition + currentRaycast.worldNormal;
        _clothColliderTransform.localPosition = new Vector3(Math.Sign(localPosition.x) * 0.25f, localPosition.y, 0);
        Solved();
    }

    private void Start() {
        _cloth = GetComponent<Cloth>();

        _clothColliderTransform = transform.Find("ClothCollider");

        SphereCollider collider = _clothColliderTransform.GetComponent<SphereCollider>();

        _clothSphereColliders = new[] { new ClothSphereColliderPair(collider) };
        _defaultSphereColliders = _cloth.sphereColliders;
    }

    private void OnBecameVisible() {
        _cloth.enabled = true;
    }

    private void OnBecameInvisible() {
        _cloth.enabled = false;
    }
}