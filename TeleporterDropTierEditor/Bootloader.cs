using System;
using BepInEx;
using BepInEx.Extensions.Configuration;
using BepInEx.Logging;
using BepInEx.Configuration;

using TeleporterDropTierEditor.Controllers;


namespace TeleporterDropTierEditor
{
    [BepInPlugin("com.maplewheels.teleporterdroptiereditor", "Teleporter Drop Tier Editor", "0.0.1")]
    [BepInDependency(BepInEx.Extensions.LibraryInfo.BepInDependencyID)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2API.Utils.NetworkCompatibility(R2API.Utils.CompatibilityLevel.EveryoneMustHaveMod)]
    public class Bootloader : BaseUnityPlugin
    {
        void Awake()
        {
            Logger.LogWarning("DEBUG: LOADING MOD");
            StaticCache<TeleporterController>.value = new TeleporterController();
            StaticCache<TeleporterController>.value.Init(Config, Logger);
        }

        void Start()
        {
            StaticCache<TeleporterController>.value.PostInit();
            StaticCache<TeleporterController>.value.Enable();
        }
    }
}
