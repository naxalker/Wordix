using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using YG;
using YG.EditorScr;

public static class PlatformSwitcher
{
    private const string PLAYGAMA_DEFINE = "PLAYGAMA";
    private const string PLUGIN_YG_DEFINE = "PLUGIN_YG_2";
    private const string PLAYGAMA_TEMPLATE = "PROJECT:Bridge";

    public static bool IsPlaygama => GetDefines().Contains(PLAYGAMA_DEFINE);
    public static bool IsPluginYourGames => GetDefines().Contains(PLUGIN_YG_DEFINE);

    public static void SwitchToPlaygama()
    {
        SwitchToWebGL();
        SetPluginYourGamesActive(false);
        SetDefines(GetDefines().Where(d => !IsPluginYourGamesDefine(d)).Append(PLAYGAMA_DEFINE).Distinct());
        PlayerSettings.WebGL.template = PLAYGAMA_TEMPLATE;
    }

    public static void SwitchToPluginYourGames()
    {
        SwitchToWebGL();
        SetDefines(GetDefines().Where(d => d != PLAYGAMA_DEFINE));
        SetPluginYourGamesActive(true);
    }

    // PluginYourGames always compiles and re-adds its defines on every project change while autoDefineSymbols is on.
    private static void SetPluginYourGamesActive(bool isActive)
    {
        InfoYG info = InfoYG.Inst();
        info.Basic.autoDefineSymbols = isActive;
        info.Basic.archivingBuild = isActive;
        EditorUtility.SetDirty(info);
        AssetDatabase.SaveAssetIfDirty(info);

        DefineSymbols.RefreshAutoDefineSubscription();
    }

    private static bool IsPluginYourGamesDefine(string define)
        => define == PLUGIN_YG_DEFINE
           || define.EndsWith("_yg", StringComparison.Ordinal)
           || define.EndsWith("_YG2", StringComparison.Ordinal);

    private static string[] GetDefines()
    {
        PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.WebGL, out string[] defines);
        return defines;
    }

    private static void SetDefines(IEnumerable<string> defines)
        => PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, defines.ToArray());

    private static void SwitchToWebGL()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
    }
}
