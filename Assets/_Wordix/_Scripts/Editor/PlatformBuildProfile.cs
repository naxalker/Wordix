using UnityEngine;
using YG.Insides;

// Project settings of a PluginYourGames platform (template, compression, ads, saves) live in its PlatformSettings asset.
public class PlatformBuildProfile : ScriptableObject
{
    [SerializeField] private PlatformSdk _sdk;
    [SerializeField] private PlatformSettings _pluginYourGamesPlatform;
    [SerializeField] private bool _zipBuild;

    public PlatformSdk Sdk => _sdk;
    public PlatformSettings PluginYourGamesPlatform => _pluginYourGamesPlatform;
    public bool ZipBuild => _zipBuild;
}
