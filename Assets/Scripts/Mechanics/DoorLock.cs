using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorLock : Riddle, IBeginDragHandler, IDragHandler {
    public Transform DoorLockTransform;

    public Texture2D RequireGlyph;
    public Activatable UnsolvedIndciator;
    public float EndPositionX;

    private Collider _collider;
    private Vector2 _lastPosition;

    public void OnBeginDrag(PointerEventData eventData) {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (_collider.Raycast(ray, out hit, Global.Constants.TouchDistance)) {
            _lastPosition = hit.textureCoord;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (RequireGlyph && LiteralPicker.Current.GlyphCode != RequireGlyph.name) return;

        if (eventData.button == PointerEventData.InputButton.Left) {
            float localPositionX = DoorLockTransform.localPosition.x;

            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            RaycastHit hit;

            if (_collider.Raycast(ray, out hit, Global.Constants.TouchDistance)) {
                Vector2 current = hit.textureCoord;

                localPositionX += current.x - _lastPosition.x;
                localPositionX = Mathf.Clamp(localPositionX, 0, EndPositionX);

                _lastPosition = current;
            }

            if (Math.Abs(localPositionX - EndPositionX) < 0.01) {
                if (UnsolvedIndciator) UnsolvedIndciator.DeActivate();

                localPositionX = EndPositionX;
                enabled = false;

                Solved();
            }

            DoorLockTransform.localPosition = new Vector3(localPositionX, 0, 0);
        }
    }

    private void Start() {
        GetComponent<Renderer>().enabled = false;

        _collider = transform.GetComponent<Collider>();
        DoorLockTransform.localPosition = Vector3.zero;
    }
}