#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class HighlightTile : FaderActivatable {

    private void Awake() {
        Fader = MaterialFader.Create(GetComponent<Renderer>(), Global.Colors.HighlightYellow, Color.black, true);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Activatable), true)]
    [CanEditMultipleObjects]
    public class HighlightEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Activate"))
                {
                    Activatable a = (Activatable)target;
                    a.Activate();
                }
                if (GUILayout.Button("Deactivate"))
                {
                    Activatable a = (Activatable)target;
                    a.DeActivate();
                }
            }
        }
    }
#endif
}
