#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class BreakableVase : Breakable {
    private string _requiredGlyph;
    public PickUp Item;
    public Transform ExplosionCenter;

    private bool _isItemPrefab;
    private Vector3 _spawnPosition;
    private Vector3 _explosionPosition;

    private new void Start() {
        base.Start();

        _requiredGlyph = GetComponentInChildren<Renderer>().ExtractGlyphName();

        if (Item != null) {
            _spawnPosition = Vector3.up * 0.25f * transform.localScale.y + transform.position;

            if (Item.gameObject.scene.rootCount == 0) {
                _isItemPrefab = true;
            }
            else {
                Item.transform.position = _spawnPosition;
                Item.gameObject.SetActive(false);
            }
        }

        _explosionPosition = ExplosionCenter != null ? ExplosionCenter.position : transform.position;
    }

    public override void OnPointerUp(PointerEventData eventData) {
        if (LiteralPicker.Current.GlyphCode != _requiredGlyph)
        {
            SoundController.instance.Play("error");
            GameControl.instance.SubtractPoint(null, null);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left) {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (!FreezeUp) {
                if (raycastResult.distance < Global.Constants.TouchDistance) {
                    Break();
                }
            }
        }
    }

    public override void Break(bool silent = false) {
        base.Break(silent);

        foreach (Rigidbody rigidbody in Rigidbodies) {
            rigidbody.AddExplosionForce(100, _explosionPosition, 1);
        }

        if (Item) {
            if (_isItemPrefab) {
                Instantiate(Item, _spawnPosition, Item.transform.rotation);
            }
            else {
                Item.gameObject.SetActive(true);
            }

            Rigidbody itemRigidbody = Item.GetComponent<Rigidbody>();
            if (itemRigidbody) {
                Vector3 toCamera = (Camera.main.transform.position - _spawnPosition).normalized;
                itemRigidbody.AddForce((Vector3.up + toCamera).normalized * 300);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BreakableVase))]
[CanEditMultipleObjects]
public class BreakableVaseEditor : Editor {

    public string BaseMaterialName = "Base";
    public string InsideMaterialName = "Black";
    public Material CurrentMaterial;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        BaseMaterialName = EditorGUILayout.TextField("Base Material Name", BaseMaterialName);
        InsideMaterialName = EditorGUILayout.TextField("Inside Material Name", InsideMaterialName);

        Material newMaterial = (Material)EditorGUILayout.ObjectField("Hieroglyph Material", CurrentMaterial, typeof(Material), false);

        if (CurrentMaterial != newMaterial) {
            CurrentMaterial = newMaterial;
            foreach (Object element in targets) {
                BreakableVase breakableVase = (BreakableVase)element;
                Renderer[] renderers = breakableVase.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers) {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++) {
                        Material material = materials[i];
                        string materialName = material.name;
                        if (!materialName.StartsWith(BaseMaterialName) && !materialName.StartsWith(InsideMaterialName)) {
                            materials[i] = newMaterial;
                        }
                    }
                    renderer.materials = materials;
                }
            }
        }
    }
}
#endif