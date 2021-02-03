using BepInEx.Configuration;
using BepInEx.Logging;

using System;
using System.Collections.Generic;
using System.Text;

namespace TeleporterDropTierEditor.Controllers
{
    class ControllerBase : IController
    {
        public bool IsEnabled { get; protected set; }

        public bool IsInit { get; protected set; }

        protected ManualLogSource Logger;
        protected ConfigFile Config;

        public void Disable()
        {
            if (!IsEnabled)
                return;

            OnDisable();
            IsEnabled = false;
        }

        public void Dispose()
        {
            if (IsEnabled)
                Disable();

            OnDispose();

            this.Config = null;
            this.Logger = null;

            this.IsInit = false;
        }

        public void Enable()
        {
            if (IsEnabled)
                return;
            if (!IsInit)
                return;

            OnEnable();
            this.IsEnabled = true;
        }

        public void Init(ConfigFile file, ManualLogSource logger)
        {
            if (IsInit)
                Dispose();

            this.Config = file;
            this.Logger = logger;

            OnInit();
            this.IsInit = true;
        }

        public void PostInit()
        {
            if (!IsInit)
                return;

            OnPostInit();
        }

        protected virtual void OnInit() { }
        protected virtual void OnPostInit() { }
        protected virtual void OnDispose() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

    }
}
