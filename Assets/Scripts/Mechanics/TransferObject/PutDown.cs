using UnityEngine;
using UnityEngine.EventSystems;

public class PutDown : Riddle, IPointerDownHandler, IPointerUpHandler {
    public Texture2D GlyphCode;
    public string RiddleCode;

    private AudioSource _audioSource;
    private Transform _target;

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (raycastResult.distance < Global.Constants.TouchDistance) {
                if (Inventory.Contains(RiddleCode)) {
                    Item item = Inventory.Item;
                    item.Putdown(_target);
                    Inventory.Clear();

                    GetComponent<Collider>().enabled = false;
                    enabled = false;

                    if (_audioSource) {
                        _audioSource.Play();
                    }

                    Solved();
                }
            }
        }
    }

    public override void Solve()
    {
        base.Solve();
        GetComponent<Collider>().enabled = false;
        enabled = false;
    }

    private void Awake() {
        if (GetComponent<MeshRenderer>()) GetComponent<MeshRenderer>().enabled = false;
        _audioSource = GetComponent<AudioSource>();

        _target = transform.Find("Target");
    }

    private void Start() {
        if (GlyphCode) {
            if (string.IsNullOrEmpty(RiddleCode)) {
                RiddleCode = GlyphCode.name;
            }
        }
        else if (string.IsNullOrEmpty(RiddleCode)) {
            Debug.LogWarning("Set 'GlyphCode' or 'RiddleCode' on " + name);
        }
    }
}