using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LookFromPoint : MonoBehaviour {

#if UNITY_EDITOR
    private void SaveLookOrigin() {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;

        SaveData(gameObject);
    }

    private void LoadLookOrigin() {
        LoadData(gameObject);
        EditorUtility.SetDirty(transform);
    }

    private void SaveData(GameObject baseObject) {
        List<string> saveData = new List<string> {
            GetInstanceID().ToString(),
            baseObject.transform.localPosition.x.ToString(),
            baseObject.transform.localPosition.y.ToString(),
            baseObject.transform.localPosition.z.ToString(),
            baseObject.transform.localRotation.eulerAngles.x.ToString(),
            baseObject.transform.localRotation.eulerAngles.y.ToString(),
            baseObject.transform.localRotation.eulerAngles.z.ToString(),
            baseObject.transform.localScale.x.ToString(),
            baseObject.transform.localScale.y.ToString(),
            baseObject.transform.localScale.z.ToString()
        };

        File.WriteAllLines(GetInstanceFileName(baseObject), saveData.ToArray());
    }

    private void LoadData(GameObject baseObject) {
        string path = GetInstanceFileName(baseObject);
        if (File.Exists(path)) {
            string[] lines = File.ReadAllLines(GetInstanceFileName(baseObject));
            if (lines.Length > 0) {
                baseObject.transform.localPosition = new Vector3(Convert.ToSingle(lines[1]), Convert.ToSingle(lines[2]), Convert.ToSingle(lines[3]));
                baseObject.transform.localRotation = Quaternion.Euler(Convert.ToSingle(lines[4]), Convert.ToSingle(lines[5]), Convert.ToSingle(lines[6]));
                baseObject.transform.localScale = new Vector3(Convert.ToSingle(lines[7]), Convert.ToSingle(lines[8]), Convert.ToSingle(lines[9]));
                File.Delete(GetInstanceFileName(baseObject));
            }
        }
        else {
            Debug.Log("No save transform found for " + baseObject.name);
        }
    }

    private string GetInstanceFileName(GameObject baseObject) {
        return string.Format(@"{0}unity\{1}_{2}.keepTransform", Path.GetTempPath(), baseObject.name, baseObject.GetInstanceID());
    }


    [CustomEditor(typeof(LookFromPoint))]
    public class LookFromEditor : Editor {

        private static LookFromPoint _lookFromPoint;
        private static bool _saved;

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            _lookFromPoint = target as LookFromPoint;

            if (Application.isPlaying) {
                if (GUILayout.Button("Set LookFrom Origin")) {
                    _lookFromPoint.SaveLookOrigin();
                    _saved = true;
                }
            }
            else if (_saved) {
                _lookFromPoint.LoadLookOrigin();
                _saved = false;
            }
        }
    }
#endif
}
