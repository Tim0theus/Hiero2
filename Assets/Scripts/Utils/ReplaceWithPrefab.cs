using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ReplaceWithPrefab : MonoBehaviour {
#if UNITY_EDITOR

    public Transform Prefab1;
    public Transform Prefab2;

    private void Editor_Replace() {
        if (!Prefab1 || !Prefab2) {
            print("No Prefab set");
            return;
        }

        HashSet<Transform> legitChildren = new HashSet<Transform>();

        foreach (Transform child in GetComponentsInChildren<Transform>()) {
            {
                if (child.name.StartsWith("simple")) {
                    legitChildren.Add(child);
                }
            }
        }

        int totalCount = legitChildren.Count;
        int counter = 0;

        print("Elements found: " + totalCount);

        foreach (Transform legitChild in legitChildren) {
            Transform prefab = Random.value >= 0.5 ? Prefab1 : Prefab2;
            Transform prefabTransform = (Transform)PrefabUtility.InstantiatePrefab(prefab);

            counter++;

            prefabTransform.name = prefab.name;
            prefabTransform.position = legitChild.position;
            prefabTransform.rotation = legitChild.rotation;
            prefabTransform.localScale = legitChild.localScale;
            prefabTransform.SetParent(legitChild.parent);

            DestroyImmediate(legitChild.gameObject);

            print("Replaced: " + counter + "/" + totalCount);
        }
    }

    [CustomEditor(typeof(ReplaceWithPrefab))]
    public class ReplaceWallWithPrefabEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUILayout.Button("Replace")) {
                ReplaceWithPrefab replace = target as ReplaceWithPrefab;
                replace.Editor_Replace();
            }
        }
    }
#endif  
}
