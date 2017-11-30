using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshCombineWizard : ScriptableWizard {
    public GameObject ParentOfObjectsToCombine;

    [MenuItem("Tools/Mesh Combine Wizard")]
    static void CreateWizard() {
        DisplayWizard<MeshCombineWizard>("Mesh Combine Wizard");
    }

    void OnWizardCreate() {
        if (ParentOfObjectsToCombine == null) return;

        Vector3 originalPosition = ParentOfObjectsToCombine.transform.position;
        ParentOfObjectsToCombine.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = ParentOfObjectsToCombine.GetComponentsInChildren<MeshFilter>();
        Dictionary<Material, List<MeshFilter>> materialToMeshFilterList = new Dictionary<Material, List<MeshFilter>>();
        List<GameObject> combinedObjects = new List<GameObject>();

        foreach (MeshFilter meshFilter in meshFilters) {
            Material[] materials = meshFilter.GetComponent<MeshRenderer>().sharedMaterials;
            if (materials == null) continue;
            if (materials.Length > 1) {
                ParentOfObjectsToCombine.transform.position = originalPosition;
                Debug.LogError("Objects with multiple materials on the same mesh are not supported. Create multiple meshes from this object's sub-meshes in an external 3D tool and assign separate materials to each.");
                return;
            }
            Material material = materials[0];
            if (materialToMeshFilterList.ContainsKey(material)) materialToMeshFilterList[material].Add(meshFilter);
            else materialToMeshFilterList.Add(material, new List<MeshFilter>() { meshFilter });
        }

        foreach (KeyValuePair<Material, List<MeshFilter>> entry in materialToMeshFilterList) {
            List<MeshFilter> meshesWithSameMaterial = entry.Value;
            string materialName = entry.Key.ToString().Split(' ')[0];

            CombineInstance[] combine = new CombineInstance[meshesWithSameMaterial.Count];
            for (int i = 0; i < meshesWithSameMaterial.Count; i++) {
                combine[i].mesh = meshesWithSameMaterial[i].sharedMesh;
                combine[i].transform = meshesWithSameMaterial[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            AssetDatabase.CreateAsset(combinedMesh, "Assets/Prefabs/Generated/Meshes/" + ParentOfObjectsToCombine.name + "_" + materialName + ".asset");

            string goName = (materialToMeshFilterList.Count > 1) ? "CombinedMesh_" + materialName : "CombinedMesh_" + ParentOfObjectsToCombine.name;
            GameObject combinedObject = new GameObject(goName);
            MeshFilter filter = combinedObject.AddComponent<MeshFilter>();
            filter.sharedMesh = combinedMesh;
            MeshRenderer renderer = combinedObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = entry.Key;
            combinedObjects.Add(combinedObject);
        }

        GameObject combindGameObject = null;
        if (combinedObjects.Count > 1) {
            combindGameObject = new GameObject(ParentOfObjectsToCombine.name + "_Combined");
            foreach (GameObject combinedObject in combinedObjects) combinedObject.transform.parent = combindGameObject.transform;
        }
        else {
            combindGameObject = combinedObjects[0];
        }

        Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Generated/" + combindGameObject.name + ".prefab");
        PrefabUtility.ReplacePrefab(combindGameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

        ParentOfObjectsToCombine.SetActive(false);
        ParentOfObjectsToCombine.transform.position = originalPosition;
        combindGameObject.transform.position = originalPosition;

        if (ParentOfObjectsToCombine.transform.parent != null) {
            combindGameObject.transform.SetParent(ParentOfObjectsToCombine.transform.parent, true);
        }

    }
}
