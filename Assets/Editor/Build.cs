using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using System;

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


public class BuildPreprocessor : MonoBehaviour, IPreprocessBuildWithReport {
    public int callbackOrder => throw new System.NotImplementedException();

    public void OnPreprocessBuild(BuildReport report) {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(VersionInfo)}");
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        VersionInfo versionInfo = AssetDatabase.LoadAssetAtPath<VersionInfo>(path);
        versionInfo.buildNumber++;
        throw new System.NotImplementedException();
    }

}