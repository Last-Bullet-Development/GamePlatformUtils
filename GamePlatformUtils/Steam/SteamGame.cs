﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformUtils.Steam
{
    //Source: https://github.com/lutris/lutris/blob/master/docs/steam.rst
    public enum SteamGameStatus
    {
        Invalid=0,
        Uninstalled=1,
        UpdateRequired=2,
        FullyInstalled=4,
        Encrypted=8,
        Locked=16,
        FilesMissing=32,
        AppRunning=64,
        FilesCorrupt=128,
        UpdateRunning=256,
        UpdatePaused=512,
        UpdateStarted=1024,
        Uninstalling=2048,
        BackupRunning=4096,
        Reconfiguring=65536,
        Validating=131072,
        AddingFiles=262144,
        Preallocating=524288,
        Downloading=1048576,
        Staging=2097152,
        Committing=4194304,
        UpdateStopping=8388608
    }

    public class SteamGame : Game
    {
        public event EventHandler StatusChanged;

        protected SteamGameStatus _Status = SteamGameStatus.Invalid;

        public new SteamGameStatus Status { get { return _Status; }
            set
            {
                bool changed = false;
                if (value != _Status)
                    changed = true;

                _Status = value;
                if (changed)
                    this.StatusChanged?.Invoke(this, new EventArgs());
            }
        }

        private string ACF;

        public SteamGame(string acf_path)
        {
            ACF = acf_path;
            this.LoadACF();
        }

        public override void Reload()
        {
            this.LoadACF();
        }

        public void LoadACF()
        {
            if (ACF == null)
                return;

            Dictionary<string, object> acf;
            using (FileStream file = new FileStream(ACF, FileMode.Open, FileAccess.Read))
                acf = new KeyValue(file).Items["appstate"] as Dictionary<string, object>;

            if (acf != null)
            {
                this.ID = acf["appid"] as string;
                this.Title = acf["name"] as string;
                this.InstallDirectory = Path.Combine(Path.GetDirectoryName(ACF), "common", acf["installdir"] as string);
                this.Size = long.Parse(acf["sizeondisk"] as string);
                this.Status = (SteamGameStatus)Enum.Parse(typeof(SteamGameStatus), acf["stateflags"] as string); ;
            }
        }

    }
}
