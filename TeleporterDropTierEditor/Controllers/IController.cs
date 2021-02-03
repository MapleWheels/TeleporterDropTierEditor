using System;
using System.Collections.Generic;
using System.Text;

namespace TeleporterDropTierEditor.Controllers
{
    interface IController
    {
        void Init(BepInEx.Configuration.ConfigFile file, BepInEx.Logging.ManualLogSource logger);
        void PostInit();
        void Enable();
        void Disable();
        void Dispose();

        bool IsEnabled { get; }
        bool IsInit { get; }
    }
}
