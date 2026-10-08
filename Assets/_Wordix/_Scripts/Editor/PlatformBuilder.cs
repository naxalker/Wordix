using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PlatformBuilder : EditorWindow
{
    private const string BUILDS_FOLDER = "Builds";

    private PlatformBuildProfile[] _profiles;
    private int _selectedIndex;

    [MenuItem("Tools/Platform Builder")]
    private static void Open() => GetWindow<PlatformBuilder>("Platform Builder");

    private void OnEnable() => LoadProfiles();

    private void OnFocus() => LoadProfiles();

    private void LoadProfiles()
    {
        _profiles = AssetDatabase.FindAssets($"t:{nameof(PlatformBuildProfile)}")
            .Select(guid => AssetDatabase.LoadAssetAtPath<PlatformBuildProfile>(AssetDatabase.GUIDToAssetPath(guid)))
            .OrderBy(profile => profile.name)
            .ToArray();

        _selectedIndex = Mathf.Clamp(_selectedIndex, 0, Mathf.Max(0, _profiles.Length - 1));
    }

    private void OnGUI()
    {
        if (_profiles.Length == 0)
        {
            EditorGUILayout.HelpBox($"No {nameof(PlatformBuildProfile)} assets found.", MessageType.Info);
            return;
        }

        PlatformBuildProfile activeProfile = _profiles.FirstOrDefault(PlatformSwitcher.IsActive);
        EditorGUILayout.LabelField("Active profile", activeProfile != null ? activeProfile.name : "None");

        _selectedIndex = EditorGUILayout.Popup("Profile", _selectedIndex, _profiles.Select(profile => profile.name).ToArray());
        PlatformBuildProfile selectedProfile = _profiles[_selectedIndex];

        using (new EditorGUI.DisabledScope(EditorApplication.isCompiling))
        {
            if (GUILayout.Button("Switch"))
                EditorApplication.delayCall += () => PlatformSwitcher.Switch(selectedProfile);

            using (new EditorGUI.DisabledScope(selectedProfile != activeProfile))
            {
                if (GUILayout.Button("Build"))
                    EditorApplication.delayCall += () => Build(selectedProfile);
            }
        }
    }

    private static void Build(PlatformBuildProfile profile)
    {
        string outputFolder = Path.Combine(BUILDS_FOLDER, profile.name, $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}");

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

        if (!profile.ZipBuild)
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
