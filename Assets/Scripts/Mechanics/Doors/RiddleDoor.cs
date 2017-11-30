#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(AnimatorActivatable))]
public class RiddleDoor : RiddleAggregator {


#if UNITY_EDITOR
    [CustomEditor(typeof(RiddleDoor))]
    [CanEditMultipleObjects]
    private class RiddleDoorEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Collect Riddles")) {

                foreach (Object element in targets) {
                    RiddleDoor riddleDoor = (RiddleDoor)element;

                    Transform riddleTransform = riddleDoor.transform.Find("Door").Find("Riddles");

                    if (riddleTransform) {
                        Riddle[] riddles = riddleTransform.GetComponentsInChildren<Riddle>();
                        riddleDoor.Riddles.Clear();

                        foreach (Riddle riddle in riddles) {
                            riddleDoor.Riddles.Add(riddle);
                        }

                        EditorUtility.SetDirty(riddleDoor);
                    }
                }
            }


        }
    }
#endif
}

