using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class ItemDropAcceptor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private Collider _collider;

    private void Start() {
        _collider = GetComponent<Collider>();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.pointerCurrentRaycast.distance < Global.Constants.TouchDistance) {

            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            RaycastHit hit;

            if (_collider.Raycast(ray, out hit, Global.Constants.TouchDistance)) {
                int layerMask = (1 << 11);
                layerMask = ~layerMask;
                Collider[] tmp = Physics.OverlapSphere(hit.point + hit.normal, 0.5f, layerMask);
                if (tmp.Length == 0)
                    Inventory.Drop(hit.point, hit.normal);
            }

        }
    }
}
