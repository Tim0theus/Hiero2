using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class ScreenLogger : MonoBehaviour {
    private static ScreenLogger _instance;

    private const int MaxQueueCount = 20;
    private static Queue<GameObject> _textObjects;

    private static int _counter;

    private void Awake() {
        _instance = this;

        _textObjects = new Queue<GameObject>();

        if (Debug.isDebugBuild) {
            Application.logMessageReceived += LogCallback;

            Clear();
            RenderTextObject("ScreenLogger online.", name, LogType.Log);
        }
    }


    public void LogVariables(params object[] inputs) {
        string output = inputs.Aggregate(string.Empty, (current, input) => current + (input + "; "));

        Debug.Log(output);
    }

    public void LogVariables(string comment, params object[] inputs) {
        string output = inputs.Aggregate(comment + ": ", (current, input) => current + (input + "; "));

        Debug.Log(output);
    }

    private void Clear() {
        while (transform.childCount > 0) {
            GameObject child = transform.GetChild(0).gameObject;
            GameObject.Destroy(child);
        }

        _textObjects.Clear();
    }

    private static void RenderTextObject(object input, string className, LogType type) {
        GameObject textObject = new GameObject("Message" + _counter++);
        textObject.transform.SetParent(_instance.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.resizeTextMinSize = 10;
        text.resizeTextMaxSize = 30;
        text.resizeTextForBestFit = true;
        text.raycastTarget = false;

        ContentSizeFitter contentSizeFitter = textObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Color textColor = Color.green;
        string logType = "Log";

        switch (type) {
            case LogType.Exception:
                textColor = Color.red;
                logType = "Exception";
                break;
            case LogType.Error:
                textColor = new Color(1, 0.5f, 0);
                logType = "Error";
                break;
            case LogType.Assert:
                textColor = Color.blue;
                logType = "Assert";
                break;
            case LogType.Warning:
                textColor = Color.yellow;
                logType = "Warning";
                break;
        }

        text.color = textColor;
        text.text = string.Format("{0:F} {1} - {2}: {3}", Time.time, className, logType, input);

        _textObjects.Enqueue(textObject);

        if (_textObjects.Count > MaxQueueCount) {
            GameObject overflow = _textObjects.Dequeue();
            GameObject.Destroy(overflow);
        }
    }

    private static void LogCallback(string condition, string stackTrace, LogType type) {
        string[] splitStackTrace = stackTrace.Split('\n');

        string classname = "";
        if (splitStackTrace.Length > 1) {
            classname = splitStackTrace[1].Split(':')[0];
        }

        RenderTextObject(condition, classname, type);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScreenLogger))]
    public class ScreenLoggerEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (Application.isPlaying && GUILayout.Button("Clear")) {
                _instance.Clear();
            }
        }
    }
#endif
}

