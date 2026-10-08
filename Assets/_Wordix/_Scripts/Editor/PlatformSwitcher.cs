using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using YG;
using YG.EditorScr;
using YG.Insides;

public static class PlatformSwitcher
{
    private const string PLAYGAMA_DEFINE = "PLAYGAMA";
    private const string PLUGIN_YG_DEFINE = "PLUGIN_YG_2";
    private const string PLAYGAMA_TEMPLATE = "PROJECT:Bridge";

    // InfoYG.SetPlatform excludes the SDKs of all platforms except the selected one from compilation.
    private const string NO_PLUGIN_YOUR_GAMES_PLATFORM = "None";

    public static bool IsActive(PlatformBuildProfile profile)
    {
        string[] defines = GetDefines();

        return profile.Sdk switch
        {
            PlatformSdk.Playgama => defines.Contains(PLAYGAMA_DEFINE),
            PlatformSdk.PluginYourGames => defines.Contains(PLUGIN_YG_DEFINE)
                                           && InfoYG.Inst().Basic.platform == profile.PluginYourGamesPlatform,
            _ => false,
        };
    }

    public static void Switch(PlatformBuildProfile profile)
    {
        SwitchToWebGL();

        if (profile.Sdk == PlatformSdk.Playgama)
            SwitchToPlaygama();
        else
            SwitchToPluginYourGames(profile.PluginYourGamesPlatform);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SwitchToPlaygama()
    {
        InfoYG info = InfoYG.Inst();
        info.Basic.platform = null;
        // PluginYourGames always compiles and re-adds its defines on every project change while autoDefineSymbols is on.
        info.Basic.autoDefineSymbols = false;
        info.Basic.archivingBuild = false;
        SaveInfo(info);

        DefineSymbols.RefreshAutoDefineSubscription();
        InfoYG.SetPlatform(NO_PLUGIN_YOUR_GAMES_PLATFORM);

        SetDefines(GetDefines().Where(d => !IsPluginYourGamesDefine(d)).Append(PLAYGAMA_DEFINE).Distinct());
        PlayerSettings.WebGL.template = PLAYGAMA_TEMPLATE;
    }

    private static void SwitchToPluginYourGames(PlatformSettings platform)
    {
        SetDefines(GetDefines().Where(d => d != PLAYGAMA_DEFINE));

        InfoYG info = InfoYG.Inst();
        info.Basic.platform = platform;
        info.Basic.autoDefineSymbols = true;
        SaveInfo(info);

        DefineSymbols.RefreshAutoDefineSubscription();
        DefineSymbols.UpdateDefineSymbols();
        platform.ApplyProjectSettings();
    }

    private static void SaveInfo(InfoYG info)
    {
        EditorUtility.SetDirty(info);
        AssetDatabase.SaveAssetIfDirty(info);
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
