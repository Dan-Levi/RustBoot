using MurkysRustBoot;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Oxide.Plugins
{

    [Info("Murkys Core", "Murky", 0.1)]
    [Description("Core plugin used in combination of RustBoot")]
    class MurkysCore : RustPlugin
    {

        Dictionary<ulong, BasePlayer> Players;

        void Init()
        {
            Players = new Dictionary<ulong, BasePlayer>();
            RustBootConsoleMsg("RustBootPluginInit");
        }

        void OnPlayerInit(BasePlayer player)
        {
            Players.Add(player.userID, player);
            RustBootConsoleMsg("PlayerConnected", JsonConvert.SerializeObject(new Player { DisplayName = player.displayName, UserID = player.userID, IpAdress = player.net.connection.ipaddress }));
        }

        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            Players.Remove(player.userID);
            RustBootConsoleMsg("PlayerDisconnected", JsonConvert.SerializeObject(new Player { DisplayName = player.displayName, UserID = player.userID, IpAdress = player.net.connection.ipaddress }));
        }

        [ConsoleCommand("RustBoot.Whisper")]
        void RustBootAdminWhisper(ConsoleSystem.Arg arg)
        {
            var args = arg.HasArgs() ? arg.Args : null;
            if (args != null && args.Length == 2)
            {
                ulong _userId;
                ulong.TryParse(args[0], out _userId);
                var message = args[1];
                if (Players.ContainsKey(_userId))
                {
                    PrintToChat(Players[_userId], message);
                }
            }
        }

        void RustBootConsoleMsg(string command, string message = "")
        {
            string json;
            if (string.IsNullOrEmpty(message))
            {
                json = JsonConvert.SerializeObject(new RustBootReport { Command = command });
            }
            else
            {
                json = JsonConvert.SerializeObject(new RustBootReport { Command = command, Message = message });
            }
            Puts("|||JSON|||" + json + "|||JSON|||");
        }
    }
}
namespace MurkysRustBoot
{
    public class Player
    {
        string displayName;
        ulong userID;
        string ipAdress;

        public string IpAdress
        {
            get { return ipAdress; }
            set { ipAdress = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        public ulong UserID
        {
            get { return userID; }
            set { userID = value; }
        }
    }
    public class RustBootReport
    {
        string command;
        string message;

        public string Command
        {
            get { return command; }
            set { command = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}