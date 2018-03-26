using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class BreakBySwipe : RiddleAggregator, IBeginDragHandler, IDragHandler {
    public Texture2D RequiredGlyph;

    private readonly HashSet<GameObject> _breakableGameObjects = new HashSet<GameObject>();
    private readonly List<RaycastResult> _results = new List<RaycastResult>();

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData) {
        if (RequiredGlyph && LiteralPicker.Current.GlyphCode != RequiredGlyph.name)
        {
            SoundController.instance.Play("error");
            GameControl.instance.SubtractPoint(null, null);
            return;
        }

        Vector3 toPlayer = (PlayerMechanics.Instance.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Dot(transform.forward, toPlayer);
        if (angleToPlayer > 0) return;

        EventSystem.current.RaycastAll(eventData, _results);

        if (_results.Count > 1) {
            GameObject raycastObject = _results[1].gameObject;

            if (raycastObject == gameObject && _results.Count > 2)
                raycastObject = _results[2].gameObject;


            if (_breakableGameObjects.Contains(raycastObject)) {
                Breakable breakable = raycastObject.GetComponent<Breakable>();
                breakable.Break();
            }
        }
    }

    private new void Awake() {
        base.Awake();

        GetComponent<Renderer>().enabled = false;

        foreach (Riddle riddle in Riddles) {
            _breakableGameObjects.Add(riddle.gameObject);
        }
    }

    public override void UpdateStatus(IObservable observable) {
        base.UpdateStatus(observable);

        if (SolvedElements == Riddles.Count) {
            GetComponent<Collider>().enabled = false;
            enabled = false;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(BreakBySwipe))]
    [CanEditMultipleObjects]
    private class BreakBySwipeEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Collect Riddles")) {

                foreach (Object element in targets) {
                    BreakBySwipe breakBySwipe = (BreakBySwipe)element;

                    Breakable[] breakables = breakBySwipe.transform.parent.GetComponentsInChildren<Breakable>();

                    breakBySwipe.Riddles.Clear();

                    foreach (Breakable breakable in breakables) {
                        breakBySwipe.Riddles.Add(breakable);
                    }

                    EditorUtility.SetDirty(breakBySwipe);
                }
            }

        }
    }
#endif
}