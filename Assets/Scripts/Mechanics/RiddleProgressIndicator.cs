#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class RiddleProgressIndicator : RiddleAggregator {
    public Transform IndicatorPrefab;
    public Material IndicatorMaterial;
    public float Offset = 0.3f;

    public override void UpdateStatus(IObservable observable) {
        Riddle riddle = (Riddle)observable;
        if (riddle.IsSolved) {
            Indicators[SolvedElements-1].Activate();
        }
        base.UpdateStatus(observable);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(RiddleProgressIndicator))]
    [CanEditMultipleObjects]
    public class RiddleProgressCounterEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate")) {
                foreach (RiddleProgressIndicator progressCounter in targets) {
                    progressCounter.Editor_Generate();
                }
            }
        }
    }

    private void Editor_Generate() {
        foreach (Activatable activatable in Indicators) {
            DestroyImmediate(activatable.transform.gameObject);
        }
        Indicators.Clear();

        for (int i = 0; i < Riddles.Count; i++) {

            Transform indicatorGlyphTransform = Instantiate(IndicatorPrefab);
            indicatorGlyphTransform.SetParent(transform, false);
            indicatorGlyphTransform.localPosition = new Vector3(Indicators.Count * Offset - (Riddles.Count - 1) * Offset / 2, 0, 0);

            Renderer indicatorGlyphRenderer = indicatorGlyphTransform.GetComponent<Renderer>();
            indicatorGlyphRenderer.material = IndicatorMaterial;

            IndicatorGlyph indicatorGlyph = indicatorGlyphTransform.GetComponent<IndicatorGlyph>();
            Indicators.Add(indicatorGlyph);
        }
    }
#endif
}