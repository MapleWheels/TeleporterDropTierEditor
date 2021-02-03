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
    class Bootloader : BaseUnityPlugin
    {
        void Awake()
        {
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
