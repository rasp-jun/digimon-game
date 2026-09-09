using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildDesktop
{
    [MenuItem("Dittoches Multi/Build Windows Client")]
    public static void Build()
    {
        PlayerSettings.productName = "Dittoches Multi";
        PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
        string output = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds", "Windows", "DittochesMulti.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/Scenes/Bootstrap.unity" },
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded) throw new BuildFailedException("Windows client build failed.");
        Debug.Log("Dittoches Multi client: " + output);
    }
}
