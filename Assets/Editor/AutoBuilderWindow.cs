using UnityEditor;
using UnityEngine;

public class AutoBuilderWindow : EditorWindow {

    [MenuItem("Build/Build Combination", false, 50)]
    public static void ShowWindow() {
        AutoBuilderWindow autoBuilderWindow = GetWindow<AutoBuilderWindow>(true, "Build Combination");
        autoBuilderWindow.minSize = new Vector2(172, 136);
        autoBuilderWindow.maxSize = autoBuilderWindow.minSize;
    }

    private bool _android;
    private bool _windows;
    private bool _ios;
    private bool _osx;
    private bool _linux;
    private bool _web;

    private void OnGUI() {

        _android = EditorGUILayout.Toggle("Android", _android);
        _windows = EditorGUILayout.Toggle("Windows", _windows);
        _ios = EditorGUILayout.Toggle("iOS", _ios);
        _osx = EditorGUILayout.Toggle("OSX", _osx);
        _linux = EditorGUILayout.Toggle("Linux", _linux);
        _web = EditorGUILayout.Toggle("WebGL", _web);


        EditorGUI.BeginDisabledGroup(!_android && !_windows && !_ios && !_osx && !_linux && !_web);
        if (GUILayout.Button("Build")) {
            Close();
            AutoBuilder.Build(_android, _windows, _ios, _osx, _linux, _web);
        }
        EditorGUI.EndDisabledGroup();
    }
}
