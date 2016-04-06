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
    }
}
