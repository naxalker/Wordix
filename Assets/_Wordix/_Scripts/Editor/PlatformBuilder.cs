using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PlatformBuilder : EditorWindow
{
    private const string BUILDS_FOLDER = "Builds";

    [MenuItem("Tools/Platform Builder")]
    private static void Open() => GetWindow<PlatformBuilder>("Platform Builder");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Current platform", GetCurrentPlatform() ?? "None");

        using (new EditorGUI.DisabledScope(EditorApplication.isCompiling))
        {
            if (GUILayout.Button("Switch to Playgama"))
                EditorApplication.delayCall += PlatformSwitcher.SwitchToPlaygama;

            if (GUILayout.Button("Switch to PluginYourGames"))
                EditorApplication.delayCall += PlatformSwitcher.SwitchToPluginYourGames;

            using (new EditorGUI.DisabledScope(GetCurrentPlatform() == null))
            {
                if (GUILayout.Button("Build"))
                    EditorApplication.delayCall += Build;
            }
        }
    }

    private static string GetCurrentPlatform()
    {
        if (PlatformSwitcher.IsPlaygama)
            return "Playgama";

        if (PlatformSwitcher.IsPluginYourGames)
            return "PluginYourGames";

        return null;
    }

    private static void Build()
    {
        string platform = GetCurrentPlatform();
        string outputFolder = Path.Combine(BUILDS_FOLDER, platform, $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}");

        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = outputFolder,
            target = BuildTarget.WebGL,
            targetGroup = BuildTargetGroup.WebGL,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            return;

        // PluginYourGames archives its builds itself.
        if (!PlatformSwitcher.IsPlaygama)
        {
            EditorUtility.RevealInFinder(outputFolder);
            return;
        }

        string zipPath = outputFolder + ".zip";
        if (File.Exists(zipPath))
            File.Delete(zipPath);

        ZipFile.CreateFromDirectory(outputFolder, zipPath);
        EditorUtility.RevealInFinder(zipPath);
    }
}
