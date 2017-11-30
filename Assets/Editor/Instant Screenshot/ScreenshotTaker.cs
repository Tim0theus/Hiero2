//C# Example

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[Serializable]
public struct ResolutionSettings {
    public string Name;
    public int Width;
    public int Height;
}


public class ScreenshotWindow : EditorWindow {

    private static MainGameView _mainGameView;
    private static DateTime _captureTime;

    public ResolutionSettings[] ResolutionSettings = { new ResolutionSettings { Width = Screen.width, Height = Screen.height, Name = Screen.width + "x" + Screen.height } };

    public Camera ScreenshotCamera;
    public Canvas MainCanvas;
    private int _scale = 1;

    private string _path = "";

    private bool _openFile;
    private bool _openDirectory;

    private bool _isTransparent;

    private ResolutionSettings _currentRenderResolution;

    private readonly List<ResolutionSettings> _screenshotsLeft = new List<ResolutionSettings>();

    [MenuItem("Tools/Instant Screenshot")]
    public static void ShowWindow() {
        ScreenshotWindow editorWindow = GetWindow<ScreenshotWindow>(true, "Instant Screenshot");
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.minSize = new Vector2(300, 600);
        editorWindow.maxSize = new Vector2(600, 2000);
    }

    private void OnGUI() {
        if (GUILayout.Button("Default Size")) {
            ResolutionSettings = new[] { new ResolutionSettings { Width = 800, Height = 600, Name = "800x600" } };
            _scale = 1;
        }

        if (GUILayout.Button("Use Presets")) {
            ResolutionSettings = new[] { new ResolutionSettings { Width = 1920, Height = 1080, Name = "AndroidPhone" } ,
                new ResolutionSettings { Width = 2560, Height = 1600, Name = "AndroidTablet" } ,
                new ResolutionSettings { Width = 2208, Height = 1242, Name = "iPhone" } ,
                new ResolutionSettings { Width = 2732, Height = 2048, Name = "iPad" } };
            _scale = 1;

            _path = "D:/HieroQuest/Output/Screenshots/Devices";
        }

        EditorGUILayout.Space();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("ResolutionSettings");

        FontStyle fontStyle = EditorStyles.label.fontStyle;
        EditorStyles.foldout.fontStyle = FontStyle.Bold;
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();
        EditorStyles.foldout.fontStyle = fontStyle;

        EditorGUILayout.Space();

        _scale = EditorGUILayout.IntSlider("Scale", _scale, 1, 10);

        EditorGUILayout.Space();

        GUILayout.Label("Save Path", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(_path);
        if (GUILayout.Button("Browse", GUILayout.MaxWidth(80))) { _path = EditorUtility.SaveFolderPanel("Path to Save Images", _path, Application.dataPath); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Choose the folder in which to save the screenshots ", MessageType.None);
        EditorGUILayout.Space();



        GUILayout.Label("Select Camera", EditorStyles.boldLabel);


        ScreenshotCamera = EditorGUILayout.ObjectField(ScreenshotCamera, typeof(Camera), true, null) as Camera ?? Camera.main;
        MainCanvas = EditorGUILayout.ObjectField(MainCanvas, typeof(Canvas), true, null) as Canvas ?? GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        _mainGameView = _mainGameView ?? new MainGameView();


        _isTransparent = EditorGUILayout.ToggleLeft("Transparent Background", _isTransparent);


        EditorGUILayout.HelpBox("Choose the camera of which to capture the render. You can make the background transparent using the transparency option.", MessageType.None);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Screenshot will be taken at", EditorStyles.boldLabel);
        foreach (ResolutionSettings resolution in ResolutionSettings) {
            EditorGUILayout.LabelField(resolution.Name + ": " + resolution.Width * _scale + " x " + resolution.Height * _scale + " px");
        }

        EditorGUILayout.Space();

        _openFile = EditorGUILayout.ToggleLeft("Open file after saving", _openFile);
        _openDirectory = EditorGUILayout.ToggleLeft("Open directory after saving", _openDirectory);

        EditorGUILayout.Space();

        if (GUILayout.Button("Take Screenshot" + (ResolutionSettings.Length > 1 ? "s" : ""), GUILayout.MinHeight(60))) {
            if (_path == "") {
                _path = EditorUtility.SaveFolderPanel("Path to Save Images", _path, Application.dataPath);
                Debug.Log("Path Set");
                TakeScreenshot();
            }
            else {
                TakeScreenshot();
            }
        }


        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Last Screenshot", GUILayout.MinHeight(40))) {
            if (LastScreenshotPath != "") {
                Application.OpenURL("file://" + LastScreenshotPath);
                Debug.Log("Opening File " + LastScreenshotPath);
            }
        }

        if (GUILayout.Button("Open Folder", GUILayout.MinHeight(40))) {

            Application.OpenURL("file://" + _path);
        }

        EditorGUILayout.EndHorizontal();

        if (_screenshotsLeft.Count > 0) {
            _currentRenderResolution = _screenshotsLeft[0];
            _screenshotsLeft.RemoveAt(0);

            float aspectRatio = (_currentRenderResolution.Width - 42) / (float)_currentRenderResolution.Height;
            _mainGameView.SetAspectRatio(aspectRatio);

            int scaledWidth = _currentRenderResolution.Width * _scale;
            int scaledHeight = _currentRenderResolution.Height * _scale;
            RenderTexture renderTexture = new RenderTexture(scaledWidth, scaledHeight, 24) { antiAliasing = 8 };
            ScreenshotCamera.targetTexture = renderTexture;

            TextureFormat tFormat = _isTransparent ? TextureFormat.ARGB32 : TextureFormat.RGB24;

            Texture2D screenshot = new Texture2D(scaledWidth, scaledHeight, tFormat, false);
            ScreenshotCamera.Render();
            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, scaledWidth, scaledHeight), 0, 0);
            ScreenshotCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenshot.EncodeToPNG();

            string filename = ScreenshotName(_currentRenderResolution.Name, scaledWidth, scaledHeight);
            string directory = string.Format(@"{0}/{1}", _path, _currentRenderResolution.Name);

            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            string path = string.Format(@"{0}/{1}", directory, filename);

            try {
                File.WriteAllBytes(path, bytes);
            }
            catch (IOException e) {
                Debug.Log(e);
            }

            Debug.Log(string.Format("Saved screenshot to: {0}", path));
            if (_openFile) { Application.OpenURL(path); }

            LastScreenshotPath = path;

            if (_openDirectory) { Application.OpenURL(_path); }
        }

    }

    private string LastScreenshotPath = "";


    private static string ScreenshotName(string prefix, int width, int height) {

        string resoluton = string.Format("{0}x{1}", width, height);

        prefix = prefix.Trim();
        prefix = string.IsNullOrEmpty(prefix) || prefix == resoluton ? string.Empty : prefix + "_";
        string strPath = string.Format("{0}{1}_{2:yyyy-MM-dd_HH-mm-ss}.png",
            prefix,
            resoluton,
            _captureTime);

        return strPath;
    }

    private void TakeScreenshot() {
        Debug.Log("Taking screenshot");
        _screenshotsLeft.AddRange(ResolutionSettings);
        _captureTime = DateTime.Now;
    }


    private class MainGameView {
        private readonly EditorWindow _gameview;

        public MainGameView() {
            Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
            MethodInfo getMainGameView = T.GetMethod("GetMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            object mainGameView = getMainGameView.Invoke(null, null);
            _gameview = (EditorWindow)mainGameView;
        }


        public void SetResolution(int width, int height) {
            Rect rect = _gameview.position;
            rect.width = width;
            rect.height = height;
            _gameview.position = rect;
        }

        public void SetAspectRatio(float ratio) {
            Rect rect = _gameview.position;
            rect.width = rect.height * ratio;
            rect.x = 300;
            rect.y = 150;
            _gameview.position = rect;
        }

        public void ForceRepaint() {
            _gameview.Repaint();
        }
    }
}
