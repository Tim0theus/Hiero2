using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CreateRigidBodyPickup : MonoBehaviour {
    public Mesh ConvexHull;
    public RiddleAggregator RiddleAggregator;

#if UNITY_EDITOR
    public GameObject Editor_ConvertPickup() {
        Transform pickupPrefab = Resources.Load<Transform>("Prefabs/Pickup");
        Transform putdownPrefab = Resources.Load<Transform>("Prefabs/Putdown");
        string riddleCode = transform.name.Split('(')[0].Trim();

        Bounds bounds = transform.GetComponentInChildren<Renderer>().bounds;
        Vector3 offset = bounds.center - transform.position;
        float largestBoundSize = Math.Max(bounds.size.x, Math.Max(bounds.size.y, bounds.size.z));
        float largestWorldScale = Math.Max(transform.lossyScale.x, Math.Max(transform.lossyScale.y, transform.lossyScale.z));
        Vector3 scale = Vector3.one * largestBoundSize / largestWorldScale;

        //Create Pickup
        Transform pickUpTransform = Instantiate(pickupPrefab, transform.parent, false);
        pickUpTransform.name = "PU:" + transform.name;
        pickUpTransform.position = transform.position + offset;
        pickUpTransform.localScale = scale;

        Transform originTransform = pickUpTransform.GetChild(0);
        originTransform.localRotation = transform.localRotation;

        RigidBodyPickUp pickUp = gameObject.AddComponent<RigidBodyPickUp>();
        pickUp.RiddleCode = riddleCode;

        //Create Putdown
        Transform putDownTransform = Instantiate(putdownPrefab, transform.parent, false);
        putDownTransform.name = "PD:" + transform.name;
        putDownTransform.position = transform.position + offset;
        putDownTransform.localScale = scale;

        Transform targetTransform = putDownTransform.GetChild(0);
        targetTransform.localRotation = transform.localRotation;

        PutDown putDown = putDownTransform.GetComponent<PutDown>();
        putDown.RiddleCode = riddleCode;

        //Create Item
        Transform itemTransform = originTransform.GetChild(0);
        Item item = itemTransform.gameObject.AddComponent<Item>();

        item.Origin = originTransform;
        pickUp.Item = item;

        //Add to parenting Riddle Aggregator
        if (RiddleAggregator == null) {
            if (transform.parent != null) {
                RiddleAggregator = transform.parent.GetComponentInParent<RiddleAggregator>();
                if (RiddleAggregator != null) {
                    RiddleAggregator.Riddles.Add(putDown);

                    EditorUtility.SetDirty(RiddleAggregator);
                }
            }
            else {
                Debug.LogWarning(name + ": No RiddleAggregator set.");
            }
        }
        else {
            RiddleAggregator.Riddles.Add(putDown);
            EditorUtility.SetDirty(RiddleAggregator);
        }

        transform.SetParent(itemTransform, true);
        pickUp.Offset = -transform.localPosition;

        //Set Colliders
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (ConvexHull) {
            meshCollider.sharedMesh = ConvexHull;
        }
        meshCollider.convex = true;

        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        return pickUpTransform.gameObject;
    }

    [CustomEditor(typeof(CreateRigidBodyPickup))]
    [CanEditMultipleObjects]
    public class CreateRigidBodyPickupEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            GUILayout.Space(20);

            if (GUILayout.Button("Convert to Pickup")) {
                GameObject[] newSelection = new GameObject[targets.Length];

                for (int i = 0; i < targets.Length; i++) {
                    CreateRigidBodyPickup createRigidBodyPickup = (CreateRigidBodyPickup)targets[i];
                    if (createRigidBodyPickup != null) {
                        newSelection[i] = createRigidBodyPickup.Editor_ConvertPickup();
                        DestroyImmediate(createRigidBodyPickup);
                    }
                }

                Selection.objects = newSelection;
            }
        }
    }
#endif
}