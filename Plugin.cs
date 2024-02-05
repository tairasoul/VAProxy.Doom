using BepInEx;

namespace DoomInProxy
{
    internal class PluginInfo
    {
        internal const string Name = "DoomIn.VAProxy";
        internal const string GUID = "tairasoul.vaproxy.doom";
        internal const string Version = "1.0.0";
    }

    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal bool init = false;
        void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {PluginInfo.GUID} ({PluginInfo.Name}) version {PluginInfo.Version} loaded.");
        }

        void Start() => Init();
        void OnDestroy() => Init();

        void Init()
        {
            if (init) return;
            init = true;
        }
    }
}
