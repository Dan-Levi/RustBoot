using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurkysRustBoot.Classes
{
    public class Settings
    {
        public bool IsRunning { get; set; } = false;
        public string Hostname { get; set; } = "A Rust server";
        public string Identity { get; set; } = "";
        public string Description { get; set; } = "This is the default server description. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. ";
        public string Headerimage { get; set; } = "";
        public string Url { get; set; } = "";
        public int Maxplayers { get; set; } = 50;
        public int Seed { get; set; } = 12345;
        public int Worldsize { get; set; } = 4000;
        public string Logfile { get; set; } = "";
        public bool RCONenabled { get; set; } = true;
        public int RCONport { get; set; } = 28016;
        public string RCONpassword { get; set; } = "supersecretpassword";
        public int Serverport { get; set; } = 28015;
        public int Tickrate { get; set; } = 30;
        public int Saveinterval { get; set; } = 600;
        public string Rustserverexecutable { get; set; } = "";
        public string Rustserverexecutablelocation { get; set; } = "";
        public string CustomArguments { get; set; }
        public string UserEvents { get; set; }
        public string BanList { get; set; }

        internal static Settings Export()
        {
            var systemSettings = Properties.Settings.Default;
            return new Settings
            {
                IsRunning = systemSettings.IsRunning,
                Hostname = systemSettings.Hostname,
                Identity = systemSettings.Identity,
                Description = systemSettings.Description,
                Headerimage = systemSettings.Headerimage,
                Url = systemSettings.Url,
                Maxplayers = systemSettings.Maxplayers,
                Seed = systemSettings.Seed,
                Worldsize = systemSettings.Worldsize,
                Logfile = systemSettings.Logfile,
                RCONenabled = systemSettings.RCONenabled,
                RCONport = systemSettings.RCONport,
                RCONpassword = systemSettings.RCONpassword,
                Serverport = systemSettings.Serverport,
                Tickrate = systemSettings.Tickrate,
                Saveinterval = systemSettings.Saveinterval,
                Rustserverexecutable = systemSettings.Rustserverexecutable,
                Rustserverexecutablelocation = systemSettings.Rustserverexecutablelocation,
                CustomArguments = systemSettings.CustomArguments,
                UserEvents = systemSettings.UserEvents,
                BanList = systemSettings.BanList
            };
        }

        internal static void Import(Settings settings)
        {
            var systemSettings = Properties.Settings.Default;
            Properties.Settings.Default.Reset();
            systemSettings.IsRunning = settings.IsRunning;
            systemSettings.Hostname = settings.Hostname;
            systemSettings.Identity = settings.Identity;
            systemSettings.Description = settings.Description;
            systemSettings.Headerimage = settings.Headerimage;
            systemSettings.Url = settings.Url;
            systemSettings.Maxplayers = settings.Maxplayers;
            systemSettings.Seed = settings.Seed;
            systemSettings.Worldsize = settings.Worldsize;
            systemSettings.Logfile = settings.Logfile;
            systemSettings.RCONenabled = settings.RCONenabled;
            systemSettings.RCONport = settings.RCONport;
            systemSettings.RCONpassword = settings.RCONpassword;
            systemSettings.Serverport = settings.Serverport;
            systemSettings.Tickrate = settings.Tickrate;
            systemSettings.Saveinterval = settings.Saveinterval;
            systemSettings.Rustserverexecutable = settings.Rustserverexecutable;
            systemSettings.Rustserverexecutablelocation = settings.Rustserverexecutablelocation;
            systemSettings.CustomArguments = settings.CustomArguments;
            systemSettings.UserEvents = settings.UserEvents;
            systemSettings.BanList = settings.BanList;
            Properties.Settings.Default.Save();
        }
    }
}
