using MurkysRustBoot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{

    [Info("Murkys Core", "Murky", 0.2)]
    [Description("Core plugin used in combination of RustBoot")]
    class MurkysCore : RustPlugin
    {

        Dictionary<ulong, BasePlayer> Players;

        void Init()
        {
            Players = new Dictionary<ulong, BasePlayer>();
            foreach (var player in BasePlayer.activePlayerList)
            {
                Players.Add(player.userID, player);
            }
            RustBootConsoleMsg("RustBootPluginInit");
        }

        /// <summary>
        /// Add player to Players dictionary
        /// </summary>
        /// <param name="player">BasePlayer</param>
        void OnPlayerInit(BasePlayer player)
        {
            if (!Players.ContainsKey(player.userID))
            {
                Players.Add(player.userID, player);
                RustBootConsoleMsg("PlayerConnected", JsonConvert.SerializeObject(new Player { DisplayName = player.displayName, UserID = player.userID, IpAdress = player.net.connection.ipaddress }));
            }
        }

        /// <summary>
        /// Remove player from Players dictionary
        /// </summary>
        /// <param name="player">BasePlayer</param>
        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            if (Players.ContainsKey(player.userID))
            {
                Players.Remove(player.userID);
                RustBootConsoleMsg("PlayerDisconnected", JsonConvert.SerializeObject(new Player { DisplayName = player.displayName, UserID = player.userID, IpAdress = player.net.connection.ipaddress }));
            }
        }

        [ConsoleCommand("RustBoot.Whisper")]
        void RustBootAdminWhisper(ConsoleSystem.Arg arg)
        {
            if (arg.connection == null)
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
        }

        /// <summary>
        /// Kill player based on Steam ID
        /// </summary>
        /// <param name="steamID">userID (Steam ID)</param>
        [ConsoleCommand("RustBoot.Kill")]
        void RustBootAdminKill(ConsoleSystem.Arg steamID)
        {
            if (steamID.connection == null)
            {
                var args = steamID.HasArgs() ? steamID.Args : null;
                if (args != null && args.Length == 1)
                {
                    ulong _userId;
                    ulong.TryParse(args[0], out _userId);
                    if (Players.ContainsKey(_userId))
                    {
                        var player = Players.FirstOrDefault(x => x.Value.userID == _userId);
                        if (player.Value != null)
                        {
                            player.Value.Die();
                        }
                    }
                }
            }
        }

        [ConsoleCommand("RustBoot.Give")]
        void RustBootAdminGive(ConsoleSystem.Arg arg)
        {
            if (arg.connection == null)
            {
                var args = arg.HasArgs() ? arg.Args : null;
                if (args != null)
                {
                    int _item;
                    ulong _userId;
                    int _amount;
                    int.TryParse(args[2], out _item);
                    if (ItemManager.itemDictionary.ContainsKey(_item))
                    {
                        ItemDefinition info = ItemManager.itemDictionary[_item];
                        ulong.TryParse(args[0], out _userId);
                        if (Players.ContainsKey(_userId))
                        {
                            var player = Players[_userId];
                            if (args.Length == 4 && args[3] == "BP")
                            {
                                if (!player.blueprints.AlreadyKnows(info))
                                {
                                    player.blueprints.Learn(info);
                                    player.ChatMessage("You have learned how to make " + info.displayName.english);
                                    return;
                                }

                            }
                            int.TryParse(args[1], out _amount);
                            if (_amount > 0)
                            {
                                player.GiveItem(ItemManager.CreateByItemID(info.itemid, _amount));
                                player.ChatMessage("You recieved " + _amount + " " + info.displayName.english + " from admin.");
                                RustBootConsoleMsg("ConsoleCommandSuccess", _amount + " " + info.displayName.english + " given to " + player.displayName);
                            }
                        }
                    }
                    else
                    {
                        RustBootConsoleMsg("ConsoleCommandError", "Item|" + _item.ToString());
                    }
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