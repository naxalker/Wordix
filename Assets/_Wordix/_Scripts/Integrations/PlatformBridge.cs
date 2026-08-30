public static class PlatformBridge
{
#if PLAYGAMA
    public static IPlatformService Service = new PlaygamaService();
#elif PLUGIN_YG_2
    public static IPlatformService Service = new PluginYourGamesService();
#endif
}
