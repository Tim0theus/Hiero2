#if UNITY_EDITOR
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer))]
public class PickUp : FaderActivatable, IPointerDownHandler, IPointerUpHandler {
    public Texture2D GlyphCode;
    public string RiddleCode;

    public Texture2D RequiredGlyph;

    public Item Item;
    public Vector3 Offset;

    private Renderer _renderer;
    private SphereCollider _triggerCollider;

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (!Inventory.HasFreeSlot || RequiredGlyph && LiteralPicker.Current.GlyphCode != RequiredGlyph.name) return;

        if (eventData.button == PointerEventData.InputButton.Left) {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (raycastResult.distance < Global.Constants.TouchDistance) {
                Pickup();
            }
        }
    }

    protected void Awake() {
        _renderer = GetComponent<Renderer>();
        _triggerCollider = GetComponent<SphereCollider>();
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

        Fader = MaterialFader.Create(GetComponent<Renderer>(), new Color(0.2f, 0.2f, 0.2f), Color.black, true, 1, true);
        Fader.Activate();
    }

    protected virtual void Pickup() {
        Item.Pickup();
        Fader.DeActivate();

        _renderer.shadowCastingMode = ShadowCastingMode.Off;
        _renderer.receiveShadows = false;
    }

    public virtual void PutDown() {
        _triggerCollider.enabled = false;

        _renderer.shadowCastingMode = ShadowCastingMode.On;
        _renderer.receiveShadows = true;
    }

    public virtual void Drop() {
        Fader.Activate();

        _renderer.shadowCastingMode = ShadowCastingMode.On;
        _renderer.receiveShadows = true;
    }
}