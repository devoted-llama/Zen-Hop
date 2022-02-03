using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System;
using UnityEditor.Build;

public class Build : MonoBehaviour {
    [MenuItem("Build/Build iOS")]
    public static void MyBuild() {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Main.unity" };
        buildPlayerOptions.locationPathName = "Build";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded) {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed) {
            Debug.Log("Build failed");
        }
    }
}

public class BuildPreprocessor : IPreprocessBuildWithReport {
    int IOrderedCallback.callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report) {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(VersionInfo)}");
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        VersionInfo versionInfo = AssetDatabase.LoadAssetAtPath<VersionInfo>(path);
      
        versionInfo.buildNumber++;
        versionInfo.dateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        versionInfo.version = PlayerSettings.bundleVersion;
        PlayerSettings.iOS.buildNumber = versionInfo.buildNumber.ToString();

        EditorUtility.SetDirty(versionInfo);
    }

}

