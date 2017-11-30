using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using Debug = UnityEngine.Debug;


public static class AutoBuilder {
    private static readonly string[] Scenes = { "Assets/Main.unity" };
    private const string OutputDirectory = @"D:\HieroQuest\Output\Build\";
    private const string ReleaseDirectory = OutputDirectory + @"Releases\";
    private const string NewestDirectory = OutputDirectory + @"Newest\";
    private const string PackageDirectory = NewestDirectory + @"Packaged\";
    private const string ZipExecutablePath = @"C:\Program Files\7-Zip\7z.exe";


    private static bool _batchMode;


    public static void Build(bool android, bool windows, bool ios, bool osx, bool linux, bool web) {
        BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string error = "";
        _batchMode = true;

        if (android) { error = BuildAndroid(); }
        if (windows && string.IsNullOrEmpty(error)) { error = BuildWindows(); }
        if (ios && string.IsNullOrEmpty(error)) { error = BuildiOS(); }
        if (osx && string.IsNullOrEmpty(error)) { error = BuildOSX(); }
        if (linux && string.IsNullOrEmpty(error)) { error = BuildLinux(); }
        if (web && string.IsNullOrEmpty(error)) { BuildWebGL(); }

        _batchMode = false;
        Process.Start(NewestDirectory);
        EditorUserBuildSettings.SwitchActiveBuildTarget(currentBuildTargetGroup, currentBuildTarget);
    }

    [MenuItem("Build/Build All", false, 0)]
    private static void BuildAll() {
        RecreateDirectory(NewestDirectory);

        Build(true, true, true, true, true, false);
    }

    [MenuItem("Build/Build Android", false, 11)]
    private static string BuildAndroid() {
        const string buildName = "Android";
        const string buildFileExtention = ".apk";

        return Build(buildName, buildFileExtention, BuildTarget.Android, true, BuildOutput.File);
    }

    [MenuItem("Build/Build Windows", false, 12)]
    private static string BuildWindows() {
        const string buildName = "Windows";
        const string buildFileExtention = ".exe";

        return Build(buildName, buildFileExtention, BuildTarget.StandaloneWindows64, true);
    }

    [MenuItem("Build/Build iOS", false, 13)]
    private static string BuildiOS() {
        //const string buildName = "iOS";
        //const string buildFileExtention = "";

        UpdateBuildVersion(BuildTarget.iOS);

        return "";
    }

    [MenuItem("Build/Build OSX", false, 14)]
    private static string BuildOSX() {
        const string buildName = "OSX";
        const string buildFileExtention = ".app";

        return Build(buildName, buildFileExtention, BuildTarget.StandaloneOSXUniversal, true, BuildOutput.Directory);
    }

    [MenuItem("Build/Build Linux", false, 15)]
    private static string BuildLinux() {
        const string buildName = "Linux";
        const string buildFileExtention = ".x86";

        return Build(buildName, buildFileExtention, BuildTarget.StandaloneLinuxUniversal, true);
    }

    [MenuItem("Build/Build WebGL", false, 16)]
    private static string BuildWebGL() {
        const string buildName = "WebGL";
        const string buildFileExtention = "";

        return Build(buildName, buildFileExtention, BuildTarget.WebGL, true, BuildOutput.Directory);
    }

    [MenuItem("Build/Player Settings", false, 80)]
    private static void OpenPlayerSettings() {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
    }

    private static string Build(string buildName, string buildFileExtention, BuildTarget buildTarget, bool updateNewestFiles, BuildOutput buildOutput = BuildOutput.FileAndDirectory) {
        string version = PlayerSettings.bundleVersion;
        BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        Debug.Log(string.Format("Building {0} {1}", buildName, version));

        string directory = buildOutput == BuildOutput.FileAndDirectory ? string.Format(@"{0}\{1}\", buildName, version) : string.Format(@"{0}\", buildName);
        string filename = string.Format(@"{0}_{1}{2}", PlayerSettings.productName, version, buildFileExtention);

        string buildPath = ReleaseDirectory + directory;

        UpdateBuildVersion(buildTarget);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
            scenes = Scenes,
            locationPathName = buildPath + filename,
            target = buildTarget,
            options = BuildOptions.None
        };

        string error = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (updateNewestFiles) {
            string newestDirectory = NewestDirectory + directory.Split('\\')[0] + @"\";
            RecreateDirectory(newestDirectory);

            switch (buildOutput) {
                case BuildOutput.File:
                    CopyFile(filename, buildPath, newestDirectory);
                    break;
                case BuildOutput.Directory:
                    CopyAllFiles(buildPath + filename, newestDirectory + filename);
                    Zip(newestDirectory, PackageDirectory);
                    break;
                case BuildOutput.FileAndDirectory:
                    CopyAllFiles(buildPath, newestDirectory);
                    Zip(newestDirectory, PackageDirectory);
                    break;
            }
        }

        Debug.Log(string.Format("Finished {0} {1}", buildName, version));

        if (!_batchMode) {
            Process.Start(NewestDirectory);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, currentBuildTarget);
        }

        return error;
    }

    private static void UpdateBuildVersion(BuildTarget buildTarget) {

        string version = PlayerSettings.bundleVersion;
        string plainVersion = new string(version.Where(char.IsDigit).ToArray());
        string buildVersion;

        switch (buildTarget) {
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                buildVersion = PlayerSettings.macOS.buildNumber;
                break;
            case BuildTarget.iOS:
                buildVersion = PlayerSettings.iOS.buildNumber;
                break;
            case BuildTarget.Android:
                buildVersion = PlayerSettings.Android.bundleVersionCode.ToString();
                break;
            default:
                return;
        }

        if (buildVersion.Length < plainVersion.Length) {
            buildVersion = plainVersion + "0";
        }

        string versionPart = buildVersion.Substring(0, plainVersion.Length);
        int buildPart = 0;
        if (versionPart == plainVersion) {
            buildPart = int.Parse(buildVersion.Substring(plainVersion.Length, buildVersion.Length - plainVersion.Length));
            buildPart++;
        }

        buildVersion = string.Format("{0}{1:D2}", plainVersion, buildPart);

        switch (buildTarget) {
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                PlayerSettings.macOS.buildNumber = buildVersion;
                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = buildVersion;
                break;
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode = int.Parse(buildVersion);
                break;
        }
    }


    private static void RecreateDirectory(string destinationPath) {
        if (Directory.Exists(destinationPath)) {
            try {
                Directory.Delete(destinationPath, true);
            }
            catch (IOException) { }
        }

        Thread.Sleep(10);
        Directory.CreateDirectory(destinationPath);
    }

    private static void CopyAllFiles(string sourcePath, string destinationPath) {
        //Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
    }

    private static void CopyFile(string filename, string sourceDirectory, string destinationDirectory) {
        Directory.CreateDirectory(destinationDirectory);
        File.Copy(sourceDirectory + filename, destinationDirectory + filename, true);
    }


    private static void Zip(string sourceDirectory, string destinationDirectory) {
        if (!File.Exists(ZipExecutablePath)) return;

        string directoryName = sourceDirectory.TrimEnd('\\').Split('\\').Last();

        ProcessStartInfo processStartInfo = new ProcessStartInfo {
            FileName = ZipExecutablePath,
            Arguments = string.Format(@"a -tzip {0}{2}_{3}_{4}.zip {1}\*", destinationDirectory, sourceDirectory, directoryName, PlayerSettings.productName, PlayerSettings.bundleVersion),
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process process = Process.Start(processStartInfo);
        process.WaitForExit();
    }

    private enum BuildOutput {
        File,
        Directory,
        FileAndDirectory
    }
}
