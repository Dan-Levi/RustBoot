using Microsoft.Win32;
using MurkysRustBoot.Classes;
using MurkysRustBoot.Properties;
using Newtonsoft.Json;
using QueryMaster;
using QueryMaster.GameServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MurkysRustBoot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Application

        public MainWindow()
        {
            InitializeComponent();
            InitApp();
        }

        private void InitApp()
        {
            CheckForDLLs();
            DataContext = this;
            Window_Settings.Visibility = Visibility.Visible;
            stack_Side_Panel_Stopped.Visibility = Visibility.Visible;
            stack_Side_Panel_Running.Visibility = Visibility.Collapsed;
            Window_Running.Visibility = Visibility.Collapsed;
            DeleteCorePlugin();
            DisablePlayerActions();
            LoadSettings();
            CheckSettings(); 
            InitUserEvents();
        }

        private void btn_Rustserverexecutable_Click(object sender, RoutedEventArgs e)
        {
            var Settings = Properties.Settings.Default;
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Rust dedicated server executable",
                DefaultExt = ".exe",
                Filter = "Windows Application (.exe)|*.exe",
                InitialDirectory = Settings.Rustserverexecutablelocation
            };
            if (fileDialog.ShowDialog() == true)
            {
                if (fileDialog.FileName.Contains("RustDedicated.exe"))
                {
                    Settings.Rustserverexecutable = fileDialog.FileName;
                    txt_Rustserverexecutable.Text = fileDialog.FileName;
                    Settings.Save();
                    CheckSettings();
                }
                else
                {
                    if (MessageBox.Show("This executable does not read RustDedicated.exe. Are you sure this is the right one?", "Correct executable?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Settings.Rustserverexecutable = fileDialog.FileName;
                        txt_Rustserverexecutable.Text = fileDialog.FileName;
                        Settings.Save();
                        CheckSettings();
                    }
                    else
                    {
                        btn_SaveSettings.IsEnabled = false;
                    }
                }
            }
        }

        private void RustBoot_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void RustBoot_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var serverRunningString = serverProcess != null ? "\nThe server currently running will be stopped." : "";
            if (MessageBox.Show("Are you sure you want to exit RustBoot?" + serverRunningString, "Exit RustBoot?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                DeleteCorePlugin();
                SaveSettings();
                if (RCONConnected && RCONCheckConnection())
                {
                    Console.WriteLine("RCON detected while restarting.");
                    RCONSendCommand("quit", true);
                }
                else
                {
                    try
                    {
                        if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
                        {
                            serverProcess.CloseMainWindow();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[RustBoot_Closing] Ops!\n" + ex.Message);
                    }
                }
            }
        }

        private void btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        bool MaximizeToggle = false;

        private void btn_Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = !MaximizeToggle ? WindowState.Maximized : WindowState.Normal;
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RustBoot_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MaximizeToggle = true;
            }
            else if (WindowState == WindowState.Normal || WindowState == WindowState.Minimized)
            {
                MaximizeToggle = false;
            }
        }

        private void btn_Report_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "mailto:post@dan-levi.no?subject=MurkysRustBoot"
            });
        }


        #endregion

        #region Settings

        void LoadSettings()
        {
            var Settings = Properties.Settings.Default;
            txt_Serverport.Text = Settings.Serverport.ToString();
            txt_Tickrate.Text = Settings.Tickrate.ToString();
            txt_Saveinterval.Text = Settings.Saveinterval.ToString();
            txt_Maxplayers.Text = Settings.Maxplayers.ToString();
            txt_Seed.Text = Settings.Seed.ToString();
            txt_Worldsize.Text = Settings.Worldsize.ToString();
            chk_RCONenabled.IsChecked = Settings.RCONenabled;
            txt_RCONport.Text = Settings.RCONport.ToString();
            txt_RCONpassword.Password = Settings.RCONpassword.ToString();
            txt_Hostname.Text = Settings.Hostname;
            txt_Description.Text = Settings.Description;
            txt_Headerimage.Text = Settings.Headerimage;
            txt_Url.Text = Settings.Url;
            txt_Rustserverexecutable.Text = Settings.Rustserverexecutable;
            txt_Customarguments.Text = Settings.CustomArguments;
        }

        void SaveSettings()
        {
            var Settings = Properties.Settings.Default;
            Settings.Serverport = txt_Serverport.Text.ToInt();
            Settings.Tickrate = txt_Tickrate.Text.ToInt();
            Settings.Saveinterval = txt_Saveinterval.Text.ToInt();
            Settings.Maxplayers = txt_Maxplayers.Text.ToInt();
            Settings.Seed = txt_Seed.Text.ToInt();
            Settings.Worldsize = txt_Worldsize.Text.ToInt();
            Settings.RCONenabled = chk_RCONenabled.IsChecked.Value;
            Settings.RCONport = txt_RCONport.Text.ToInt();
            Settings.RCONpassword = txt_RCONpassword.Password;
            Settings.Hostname = txt_Hostname.Text;
            Settings.Identity = txt_Hostname.Text.Sanitize();
            Settings.Description = txt_Description.Text;
            Settings.Headerimage = txt_Headerimage.Text;
            Settings.Url = txt_Url.Text;
            Settings.Rustserverexecutable = txt_Rustserverexecutable.Text;
            try
            {
                var serverPath = Path.Combine(Path.GetDirectoryName(Settings.Rustserverexecutable), "server", Settings.Identity);
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SaveSettings] Ops!\n" + ex.Message);
            }
            Settings.CustomArguments = txt_Customarguments.Text;
            Settings.Save();
        }

        void DefaultSettings()
        {
            tab_Plugins.IsEnabled = false;
            btn_SaveSettings.IsEnabled = false;
            var DefaultSettings = new Classes.Settings();
            var Settings = Properties.Settings.Default;
            Settings.Serverport = DefaultSettings.Serverport;
            Settings.Tickrate = DefaultSettings.Tickrate;
            Settings.Saveinterval = DefaultSettings.Saveinterval;
            Settings.Maxplayers = DefaultSettings.Maxplayers;
            Settings.Seed = DefaultSettings.Seed;
            Settings.Worldsize = DefaultSettings.Worldsize;
            Settings.RCONenabled = DefaultSettings.RCONenabled;
            Settings.RCONport = DefaultSettings.RCONport;
            Settings.RCONpassword = DefaultSettings.RCONpassword;
            Settings.Hostname = DefaultSettings.Hostname;
            Settings.Identity = DefaultSettings.Identity;
            Settings.Description = DefaultSettings.Description;
            Settings.Headerimage = DefaultSettings.Headerimage;
            Settings.Url = DefaultSettings.Url;
            Settings.Rustserverexecutable = DefaultSettings.Rustserverexecutable;
            Settings.Rustserverexecutablelocation = DefaultSettings.Rustserverexecutablelocation;
            Settings.CustomArguments = DefaultSettings.CustomArguments;
            Settings.Save();
            btn_StartServer.IsEnabled = false;
            LoadSettings();
        }

        private void btn_SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Settings Saved.");
            CheckSettings();
        }

        private void btn_ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to load default settings?", "Load default settings?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DefaultSettings();
                MessageBox.Show("Default settings loaded.");
            }
        }

        private void CheckSettings()
        {
            var Settings = Properties.Settings.Default;
            if (!string.IsNullOrEmpty(Settings.Rustserverexecutable))
            {
                if (!File.Exists(Settings.Rustserverexecutable))
                {
                    Console.WriteLine("[CheckSettings] Ops!\n" + "Executable not present!");
                }
                tab_Plugins.IsEnabled = true;
                btn_SaveSettings.IsEnabled = true;
                if (!string.IsNullOrEmpty(Settings.Hostname) && Settings.Maxplayers > 0 && Settings.Worldsize > 0 && Settings.Seed.ToString().Length == 5)
                {
                    CheckServerData(Settings);
                    if (Settings.RCONenabled)
                    {
                        if (Settings.RCONport == 0 || string.IsNullOrEmpty(Settings.RCONpassword))
                        {
                            Settings.RCONpassword = Randomstring(6);
                        }
                        else
                        {
                            if (Settings.Serverport != 0 && Settings.Tickrate != 0 && Settings.Saveinterval != 0)
                            {
                                btn_StartServer.IsEnabled = true;
                            }
                        }
                    }
                    if (Settings.Serverport != 0 && Settings.Tickrate != 0 && Settings.Saveinterval != 0)
                    {
                        btn_StartServer.IsEnabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Error detected in General tab.\nCheck that seed is 5 digits.\nRevert to defaults if in doubt.");
                    btn_StartServer.IsEnabled = false;
                }
            }
            else
            {
                tab_Plugins.IsEnabled = false;
                btn_StartServer.IsEnabled = false;
                btn_Rustserverexecutable_Click(btn_Rustserverexecutable as object, new RoutedEventArgs());
            }
        }

        private void CheckServerData(Properties.Settings Settings)
        {
            var serverFolder = Path.GetDirectoryName(Settings.Rustserverexecutable);
            if (!string.IsNullOrEmpty(Settings.Identity))
            {
                var identityFolder = Path.Combine(serverFolder, "server", Settings.Identity);
                if (Directory.Exists(identityFolder))
                {
                    if (File.Exists(Path.Combine(identityFolder, "Storage.db")) || File.Exists(Path.Combine(identityFolder, "UserPersistence.db")))
                    {
                        btn_BP_Wipe.IsEnabled = true;
                    }
                    else
                    {
                        btn_BP_Wipe.IsEnabled = false;
                    }
                    if (File.Exists(Path.Combine(identityFolder, "Log.EAC.txt")) || File.Exists(Path.Combine(identityFolder, "Log.Log.txt")) || File.Exists(Path.Combine(identityFolder, "Log.Warning.txt")))
                    {
                        btn_Logs_Delete.IsEnabled = true;
                    }
                    else
                    {
                        btn_Logs_Delete.IsEnabled = false;
                    }
                    if (Directory.Exists(Path.Combine(identityFolder, "save")))
                    {
                        if (Directory.EnumerateFileSystemEntries(Path.Combine(identityFolder, "save")).Any())
                        {
                            btn_Map_Wipe.IsEnabled = true;
                        }
                        else
                        {
                            btn_Map_Wipe.IsEnabled = false;
                        }
                    }
                    else
                    {
                        btn_Map_Wipe.IsEnabled = false;
                    }
                }
            }
        }




        #endregion

        #region GUI

        private void btn_Seed_Click(object sender, RoutedEventArgs e)
        {
            txt_Seed.Text = (new Random()).Next(10000, 99999).ToString("D5");
        }

        void SwitchWindowLayout(ServerState layoutType)
        {
            switch (layoutType)
            {
                case ServerState.RUNNING:
                    Window_Settings.Visibility = Visibility.Collapsed;
                    btn_Rustserverexecutable.IsEnabled = false;
                    btn_SaveSettings.IsEnabled = false;
                    btn_ResetSettings.IsEnabled = false;
                    lbl_Serverexecutable.Visibility = Visibility.Collapsed;
                    txt_Rustserverexecutable.Visibility = Visibility.Collapsed;
                    Window_Running.Visibility = Visibility.Visible;
                    stack_Side_Panel_Running.IsEnabled = true;
                    stack_Side_Panel_Running.Visibility = Visibility.Visible;
                    stack_Side_Panel_Stopped.IsEnabled = false;
                    stack_Side_Panel_Stopped.Visibility = Visibility.Collapsed;
                    break;
                case ServerState.STOPPED:
                    Window_Settings.Visibility = Visibility.Visible;
                    btn_Rustserverexecutable.IsEnabled = true;
                    btn_SaveSettings.IsEnabled = true;
                    btn_ResetSettings.IsEnabled = true;
                    lbl_Serverexecutable.Visibility = Visibility.Visible;
                    txt_Rustserverexecutable.Visibility = Visibility.Visible;
                    Window_Running.Visibility = Visibility.Collapsed;
                    stack_Side_Panel_Running.IsEnabled = false;
                    stack_Side_Panel_Running.Visibility = Visibility.Collapsed;
                    stack_Side_Panel_Stopped.IsEnabled = true;
                    stack_Side_Panel_Stopped.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void btn_Map_Wipe_Click(object sender, RoutedEventArgs e)
        {
            var Settings = Properties.Settings.Default;
            if (MessageBox.Show("Are you sure you want to wipe Map?", "Wipe map?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var serverFolder = Path.GetDirectoryName(Settings.Rustserverexecutable);
                var mapFolder = Path.Combine(serverFolder, "server", Settings.Identity, "save");
                if (Directory.Exists(mapFolder))
                {
                    try
                    {
                        var mapFolderInfo = new DirectoryInfo(mapFolder);
                        foreach (var file in mapFolderInfo.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (var folder in mapFolderInfo.GetDirectories())
                        {
                            folder.Delete(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[btn_Map_Wipe_Click] Ops!\n" + ex.Message);
                        MessageBox.Show("Could not wipe map.\n Some files in " + mapFolder + " may be open in another application", "Could not delete map data");
                    }
                    finally
                    {
                        btn_Map_Wipe.IsEnabled = false;
                        MessageBox.Show("Map wiped.");
                    }
                }
            }
        }

        private void btn_BP_Wipe_Click(object sender, RoutedEventArgs e)
        {
            var Settings = Properties.Settings.Default;
            if (MessageBox.Show("Are you sure you want to wipe blueprints?", "Wipe blueprints?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var identityFolder = Path.Combine(Path.GetDirectoryName(Settings.Rustserverexecutable), "server", Settings.Identity);
                try
                {
                    var identityFolderInfo = new DirectoryInfo(identityFolder);
                    foreach (var file in identityFolderInfo.GetFiles())
                    {
                        if (file.Name.Contains("Storage.db") || file.Name.Contains("UserPersistence.db"))
                        {
                            file.Delete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[btn_BP_Wipe_Click] Ops!\n" + ex.Message);
                }
                finally
                {
                    btn_BP_Wipe.IsEnabled = false;
                    MessageBox.Show("Blueprints wiped.");
                }
            }
        }

        private void btn_Logs_Delete_Click(object sender, RoutedEventArgs e)
        {
            var Settings = Properties.Settings.Default;
            if (MessageBox.Show("Are you sure you want to delete the logs?", "Delete logs?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var identityFolder = Path.Combine(Path.GetDirectoryName(Settings.Rustserverexecutable), "server", Settings.Identity);
                try
                {
                    var identityFolderInfo = new DirectoryInfo(identityFolder);
                    foreach (var file in identityFolderInfo.GetFiles())
                    {
                        if (file.Name.Contains("Log.") && file.Name.EndsWith(".txt"))
                        {
                            file.Delete();
                        }
                        txt_Console.Clear();
                        PrintToGeneralLog("Logs cleared.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[btn_Logs_Delete_Click] Ops!\n" + ex.Message);
                }
                finally
                {
                    btn_Logs_Delete.IsEnabled = false;
                    MessageBox.Show("Logs deleted.");
                }
            }
        }

        #endregion

        #region User32

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        #endregion

        #region Players

        private void InitiatePlayerList()
        {
            Players = new ObservableCollection<Player>();
            list_Players.ItemsSource = Players;
        }

        private void AddPlayer(string json)
        {
            Player player = JsonConvert.DeserializeObject<Player>(json);
            Console.WriteLine("Player added: " + player.DisplayName);
            Players.Add(player);
        }

        private void RemovePlayer(string json)
        {
            Player player = JsonConvert.DeserializeObject<Player>(json);
            var client = _players.FirstOrDefault(i => i.UserID == player.UserID);
            if (client != null)
            {
                _players.Remove(client);
            }
        }

        private void EnablePlayerActions()
        {
            Console.WriteLine("Chat is now private with " + ChatSessionPlayer.DisplayName);

            btn_Ban_Player.IsEnabled = true;
            btn_Ban_Player.Visibility = Visibility.Visible;

            btn_Kick_Player.IsEnabled = true;
            btn_Kick_Player.Visibility = Visibility.Visible;

            btn_Moderator_Player.IsEnabled = true;
            btn_Moderator_Player.Visibility = Visibility.Visible;

            btn_Unmoderator_Player.IsEnabled = true;
            btn_Unmoderator_Player.Visibility = Visibility.Visible;

            btn_Give_Player.IsEnabled = true;
            btn_Give_Player.Visibility = Visibility.Visible;

            btn_Kill_Player.IsEnabled = true;
            btn_Kill_Player.Visibility = Visibility.Visible;
        }

        private void DisablePlayerActions()
        {
            btn_Ban_Player.IsEnabled = false;
            btn_Ban_Player.Visibility = Visibility.Collapsed;

            btn_Kick_Player.IsEnabled = false;
            btn_Kick_Player.Visibility = Visibility.Collapsed;

            btn_Moderator_Player.IsEnabled = false;
            btn_Moderator_Player.Visibility = Visibility.Collapsed;

            btn_Unmoderator_Player.IsEnabled = false;
            btn_Unmoderator_Player.Visibility = Visibility.Collapsed;

            btn_Give_Player.IsEnabled = false;
            btn_Give_Player.Visibility = Visibility.Collapsed;

            btn_Kill_Player.IsEnabled = false;
            btn_Kill_Player.Visibility = Visibility.Collapsed;
        }

        ObservableCollection<Player> _players;
        public ObservableCollection<Player> Players
        {
            get { return _players; }
            set { _players = value; }
        }

        private void list_Players_LayoutUpdated(object sender, EventArgs e)
        {
            txt_Playercount.Text = _players != null ? _players.Count.ToString() : txt_Playercount.Text;
        }

        private void list_Players_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisablePlayerActions();
            if (e.Source is ListBox)
            {
                Player player = (sender as ListBox).SelectedItem as Player;
                if (player != null)
                {
                    txt_Players_Info_Username.Text = player.DisplayName;
                    txt_Players_Info_UserID.Text = player.UserID.ToString();
                    txt_Players_Info_IP_Adress.Text = player.IpAdress;
                    chatMode = ChatMode.Private;
                    ConsoleInputMode(ChatMode.Private);
                    ChatSessionPlayer = player;
                    EnablePlayerActions();
                }
            }
        }

        private void btn_Kick_Player_Click(object sender, RoutedEventArgs e)
        {
            Player player = list_Players.SelectedItem as Player;
            if (player != null)
            {
                if (MessageBox.Show("Kick " + player.DisplayName + "?", "Kick player?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (RCONConnected && RCONCheckConnection())
                    {
                        RCONSendCommand("kick " + player.UserID + " \"" + player.DisplayName + "\"", true);
                        Players.Remove(player);
                    }
                }
            }
        }

        private void btn_Ban_Player_Click(object sender, RoutedEventArgs e)
        {
            Player player = list_Players.SelectedItem as Player;
            if (player != null)
            {
                if (MessageBox.Show("Ban " + player.DisplayName + "?", "Ban player?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (RCONConnected && RCONCheckConnection())
                    {
                        RCONSendCommand("ban " + player.UserID + " \"" + player.DisplayName + "\"", true);
                        Players.Remove(player);
                    }
                }
            }
        }

        private void btn_Moderator_Player_Click(object sender, RoutedEventArgs e)
        {
            Player player = list_Players.SelectedItem as Player;
            if (player != null)
            {
                if (MessageBox.Show("Make " + player.DisplayName + " moderator?", "Make player moderator?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (RCONConnected && RCONCheckConnection())
                    {
                        RCONSendCommand("moderatorid " + player.UserID + " \"" + player.DisplayName + "\"", true);
                        RCONSendCommand("kick " + player.UserID + " \"" + player.DisplayName + "\" " + "\"" + "moderation privileges granted.\"");
                        Players.Remove(player);
                    }
                }
            }
        }

        private void btn_Unmoderator_Player_Click(object sender, RoutedEventArgs e)
        {
            Player player = list_Players.SelectedItem as Player;
            if (player != null)
            {
                if (MessageBox.Show("Remove moderation privileges for " + player.DisplayName + "?", "Remove moderation privileges?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (RCONConnected && RCONCheckConnection())
                    {
                        RCONSendCommand("removemoderator " + player.UserID, true);
                        RCONSendCommand("kick " + player.UserID + " \"" + player.DisplayName + "\" " + "\"" + "moderation privileges revoked by admin.\"");
                        Players.Remove(player);
                    }
                }
            }
        }

        private void btn_Give_Player_Click(object sender, RoutedEventArgs e)
        {
            Player player = list_Players.SelectedItem as Player;
            if (player != null)
            {
                ItemPicker ip = new ItemPicker();
                ip.Owner = this;
                Opacity = 0.4;
                if (ip.ShowDialog() == true)
                {
                    Opacity = 1;
                    if (ip.RustItemsToGive != null)
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            foreach (RustItem rustItemToGive in ip.RustItemsToGive)
                            {
                                if (rustItemToGive.IsBP)
                                {
                                    RCONSendCommand("RustBoot.Give " + player.UserID + " " + 0 + " " + rustItemToGive.ID + " BP", true);
                                }
                                else
                                {
                                    RCONSendCommand("RustBoot.Give " + player.UserID + " " + rustItemToGive.Amount.ToString() + " " + rustItemToGive.ID, true);
                                }
                                Thread.Sleep(2000);
                            }
                        }).Start();
                    }
                }
            }
        }

        private void btn_Kill_Player_Click(object sender, RoutedEventArgs e)
        {
            Player player = list_Players.SelectedItem as Player;
            if (player != null)
            {
                if (MessageBox.Show("Really kill " + player.DisplayName + "?", "Kill player?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    RCONSendCommand("RustBoot.Kill " + player.UserID.ToString(), true);
                }
            }
        }

        #endregion

        #region Events

        public ObservableCollection<UserEvent> UserEvents;

        void InitUserEvents()
        {
            
            btn_User_Event_Add.IsEnabled = false;
            UserEvents = new ObservableCollection<UserEvent>();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.UserEvents))
            {
                try
                {
                    JsonConvert.DeserializeObject<List<UserEvent>>(Properties.Settings.Default.UserEvents).ForEach(x =>
                    {
                        if (x != null)
                        {
                            UserEvents.Add(x);
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("[InitUserEvents] Ops!\n" +ex.Message);
                }
            }
            else
            {
                UserEvents.Add(new UserEvent("Message of the day",60, "We are having a <color=\"green\">great day</color> saying hello every minute!", "Chatmessage"));
            }
            list_User_Events.ItemsSource = UserEvents;
            UserEvents.CollectionChanged += UserEvents_CollectionChanged;
        }

        private void UserEvents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(UserEvents.Cast<UserEvent>().ToList());
            Properties.Settings.Default.UserEvents = json;
            Properties.Settings.Default.Save();
        }

        private void CheckEventCompleted(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                CheckEventIsValid();
            }
            else if (sender is ComboBox)
            {
                txt_User_Event_Command_Label.Text = combo_User_Event_Type.Text + ": ";
                txt_User_Event_Command.Height = combo_User_Event_Type.Text != "Chatmessage" ? 20 : 60;
                btn_User_Event_Add.Height = combo_User_Event_Type.Text != "Chatmessage" ? 20 : 40;
                CheckEventIsValid();
            }
        }

        UserEvent userEvent;

        void CheckEventIsValid()
        {
            if (!string.IsNullOrEmpty(txt_User_Event_Name.Text) && !string.IsNullOrEmpty(txt_User_Event_Interval.Text) && !string.IsNullOrEmpty(txt_User_Event_Command.Text) && txt_User_Event_Interval.Text.ToInt() >= 5)
            {
                userEvent = new UserEvent
                (
                    txt_User_Event_Name.Text,
                    txt_User_Event_Interval.Text.ToInt(),
                    txt_User_Event_Command.Text,
                    combo_User_Event_Type.Text
                );
                btn_User_Event_Add.IsEnabled = true;
            }
            else
            {
                try
                {
                    btn_User_Event_Add.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[CheckEventIsValid] Ops!\n" + ex.Message);
                }
            }
        }

        private void btn_User_Event_Add_Click(object sender, RoutedEventArgs e)
        {
            if (userEvent != null)
            {

                UserEvents.Add(userEvent);
            }
        }

        void RunUserEvents()
        {
            foreach (var userEvent in UserEvents)
            {
                System.Timers.Timer timer = new System.Timers.Timer {
                    Interval = ((double)userEvent.Interval * 1000),
                    AutoReset = true
                };
                var commandPrefix = userEvent.EventType == "Chatmessage" ? "say " : "";
                timer.Elapsed += (o, e) =>
                {
                    if (RCONConnected)
                    {
                        RCONSendCommand(commandPrefix + userEvent.Command, true);
                    }
                };
                timer.Start();
            }
        }


        private void btn_User_Event_Type_Info_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://oxidemod.org/threads/server-commands-for-rust.6404/");
        }

        private void btn_list_user_Event_Delete_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            UserEvent userEvent = btn.DataContext as UserEvent;
            if (userEvent != null)
            {
                UserEvents.Remove(userEvent);
            }
        }

        #endregion

        #region Plugins

        string PluginDirectory;
        string DeactivatedPluginsDirectory;
        List<string> Plugins;
        List<string> DeactivatedPlugins;
        List<string> ValidPluginExtensions = new List<string> {
            ".cs",".lua",".py"
        };
        bool ValidDrop = false;
        ListBox LastManipulatedListBox;

        private void CheckForCorePlugin()
        {
            PluginDirectory = Path.Combine(new string[] {
                Path.GetDirectoryName(Properties.Settings.Default.Rustserverexecutable),
                "server",
                Properties.Settings.Default.Identity,
                "oxide",
                "plugins" });
            AddCorePlugin();
        }

        private void AddCorePlugin()
        {
            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);
            if (!File.Exists(Path.Combine(PluginDirectory, "MurkysCore.cs")))
            {
                CopyEmbeddedResource("Plugins.MurkysCore.cs", Path.Combine(PluginDirectory, "MurkysCore.cs"));
            }
        }

        private void DeleteCorePlugin()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Rustserverexecutable) && !string.IsNullOrEmpty(Properties.Settings.Default.Identity))
            {
                PluginDirectory = Path.Combine(new string[] {
                    Path.GetDirectoryName(Properties.Settings.Default.Rustserverexecutable),
                    "server",
                    Properties.Settings.Default.Identity,
                    "oxide",
                    "plugins" });
                if (File.Exists(Path.Combine(PluginDirectory, "MurkysCore.cs")))
                {
                    File.Delete(Path.Combine(PluginDirectory, "MurkysCore.cs"));
                }
            }
        }

        private void ParseCorePluginMessage(string v)
        {
            var json = v.Split(new string[] { "|||JSON|||" }, StringSplitOptions.RemoveEmptyEntries)[1];
            RustBootReport report = JsonConvert.DeserializeObject<RustBootReport>(json);
            if (IsRunning)
            {
                switch (report.Command)
                {
                    case "RustBootPluginInit":
                        break;
                    case "PlayerConnected":
                        AddPlayer(report.Message);
                        break;
                    case "PlayerDisconnected":
                        Console.WriteLine("Player Disconnected");
                        RemovePlayer(report.Message);
                        break;
                    default:
                        break;
                }
            }
        }

        private void RefreshPluginLists()
        {
            var Settings = Properties.Settings.Default;
            Plugins = new List<string>();
            DeactivatedPlugins = new List<string>();

            if (Settings.Identity != txt_Hostname.Text.Sanitize())
            {
                Settings.Identity = txt_Hostname.Text.Sanitize();
                Settings.Save();
            }
            PluginDirectory = Path.Combine(new string[] {
                Path.GetDirectoryName(Properties.Settings.Default.Rustserverexecutable),
                "server",
                Settings.Identity,
                "oxide",
                "plugins" });
            DeactivatedPluginsDirectory = Path.Combine(new string[] {
                Path.GetDirectoryName(Properties.Settings.Default.Rustserverexecutable),
                "server",
                Settings.Identity,
                "oxide",
                "deactivated_plugins" });

            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);
            if (!Directory.Exists(DeactivatedPluginsDirectory))
                Directory.CreateDirectory(DeactivatedPluginsDirectory);

            list_Plugins.Items.Clear();
            list_DeactivatedPlugins.Items.Clear();
            foreach (var file in Directory.GetFiles(PluginDirectory))
            {
                Plugins.Add(Path.GetFileName(file));
                list_Plugins.Items.Add(Path.GetFileName(file));
            }
            foreach (var file in Directory.GetFiles(DeactivatedPluginsDirectory))
            {
                DeactivatedPlugins.Add(Path.GetFileName(file));
                list_DeactivatedPlugins.Items.Add(Path.GetFileName(file));
            }
            txt_PluginsDragMessage.Visibility = list_Plugins.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void list_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender as ListBox != LastManipulatedListBox && LastManipulatedListBox != null)
            {
                LastManipulatedListBox.SelectedItems.Clear();
            }
            LastManipulatedListBox = sender as ListBox;
            if (LastManipulatedListBox != null && LastManipulatedListBox.Items.Count > 0 && LastManipulatedListBox.SelectedIndex != -1)
            {
                btn_TogglePluginActivation.IsEnabled = true;
                btn_DeletePlugins.IsEnabled = true;
            }
            else
            {
                btn_TogglePluginActivation.IsEnabled = false;
                btn_DeletePlugins.IsEnabled = false;
            }
        }

        private void btn_DeletePlugins_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected plugin(s)?", "Delete selected plugins?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (string fileName in LastManipulatedListBox.SelectedItems)
                {
                    if (LastManipulatedListBox == list_Plugins)
                    {
                        if (File.Exists(Path.Combine(PluginDirectory, fileName)))
                        {
                            File.Delete(Path.Combine(PluginDirectory, fileName));
                        }
                    }
                    else if (LastManipulatedListBox == list_DeactivatedPlugins)
                    {
                        if (File.Exists(Path.Combine(DeactivatedPluginsDirectory, fileName)))
                        {
                            File.Delete(Path.Combine(DeactivatedPluginsDirectory, fileName));
                        }
                    }
                }
                RefreshPluginLists();
            }
        }

        private void btn_TogglePluginActivation_Click(object sender, RoutedEventArgs e)
        {
            if (LastManipulatedListBox != null && LastManipulatedListBox.Items.Count > 0 && LastManipulatedListBox.SelectedIndex != -1)
            {
                if (LastManipulatedListBox == list_Plugins)
                {
                    foreach (string fileName in list_Plugins.SelectedItems)
                    {
                        if (File.Exists(Path.Combine(PluginDirectory, fileName)))
                        {
                            File.Move(Path.Combine(PluginDirectory, fileName), Path.Combine(DeactivatedPluginsDirectory, fileName));
                        }
                    }
                }
                else if (LastManipulatedListBox == list_DeactivatedPlugins)
                {
                    foreach (string fileName in list_DeactivatedPlugins.SelectedItems)
                    {
                        if (File.Exists(Path.Combine(DeactivatedPluginsDirectory, fileName)))
                        {
                            File.Move(Path.Combine(DeactivatedPluginsDirectory, fileName), Path.Combine(PluginDirectory, fileName));
                        }
                    }
                }
                RefreshPluginLists();
            }
        }

        private void list_Plugins_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var listBox = (sender as ListBox) == list_Plugins ? list_Plugins : list_DeactivatedPlugins;
                txt_PluginsDragMessage.Visibility = Visibility.Visible;
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool isValidFileExtensions = true;
                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (!ValidPluginExtensions.Contains(Path.GetExtension(filePaths[i])))
                    {
                        isValidFileExtensions = false;
                        break;
                    }
                }
                if (!isValidFileExtensions)
                {
                    ValidDrop = false;
                    e.Effects = DragDropEffects.None;
                    listBox.BorderBrush = GetColorBrush("DangerRed");
                    txt_PluginsDragMessage.Foreground = GetColorBrush("DangerRed");
                    txt_PluginsDragMessage.Text = "Invalid file(s)";
                }
                else
                {
                    ValidDrop = true;
                    e.Effects = DragDropEffects.Copy;
                    listBox.BorderBrush = GetColorBrush("SuccessGreen");
                    txt_PluginsDragMessage.Foreground = GetColorBrush("SuccessGreen");
                    txt_PluginsDragMessage.Text = "Drop to import";
                }
            }
        }

        private void list_Plugins_DragLeave(object sender, DragEventArgs e)
        {
            var listBox = (sender as ListBox) == list_Plugins ? list_Plugins : list_DeactivatedPlugins;
            listBox.BorderBrush = GetColorBrush("BorderDarkGreen");
            txt_PluginsDragMessage.Foreground = new SolidColorBrush(Colors.White);
            txt_PluginsDragMessage.Text = "Drag and drop";
        }

        private void list_Plugins_Drop(object sender, DragEventArgs e)
        {
            var listBox = (sender as ListBox) == list_Plugins ? list_Plugins : list_DeactivatedPlugins;
            if (ValidDrop && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var pluginDir = listBox == list_Plugins ? PluginDirectory : DeactivatedPluginsDirectory;
                var otherPluginDir = listBox == list_Plugins ? DeactivatedPluginsDirectory : PluginDirectory;
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    var filePath = filePaths[i];
                    var fileName = Path.GetFileName(filePaths[i]);
                    if (File.Exists(Path.Combine(pluginDir, fileName)))
                    {
                        if (MessageBox.Show("Overwrite " + fileName + "?\nClicking no will skip this file", "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            File.Copy(filePath, Path.Combine(pluginDir, fileName), true);
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(otherPluginDir, fileName)))
                        {
                            var listName = listBox == list_Plugins ? "deactivated plugins" : "activated plugins";
                            MessageBox.Show(fileName + " is already listed in " + listName + ". skipping this file.", "Plugin exists");
                        }
                        else
                        {
                            File.Copy(filePath, Path.Combine(pluginDir, fileName));
                        }
                    }
                }
                RefreshPluginLists();
                txt_PluginsDragMessage.Visibility = Visibility.Collapsed;
                listBox.BorderBrush = GetColorBrush("BorderDarkGreen");
            }
            else
            {
                txt_PluginsDragMessage.Visibility = Visibility.Collapsed;
                listBox.BorderBrush = GetColorBrush("BorderDarkGreen");
            }
        }

        #endregion

        #region Logs

        static Dictionary<string, string> LogFiles = new Dictionary<string, string>
        {
            {"Log.Chat.txt","Chat"},
            {"Log.EAC.txt","EAC"},
            {"Log.Error.txt","Error"},
            {"Log.Log.txt","General"},
            {"Log.Warning.txt","Warning"}
        };

        static Dictionary<string, int> LogFilesReadCount = new Dictionary<string, int>
        {
            {"Log.Chat.txt",0},
            {"Log.EAC.txt",0},
            {"Log.Error.txt",0},
            {"Log.Log.txt",0},
            {"Log.Warning.txt",0}
        };

        void InitiateLogWatchers()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.Rustserverexecutable), "server", Properties.Settings.Default.Identity);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
       | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.txt";
            watcher.Changed += new FileSystemEventHandler(LogFilesOnChanged);
            watcher.Created += new FileSystemEventHandler(LogFilesOnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void LogFilesOnChanged(object sender, FileSystemEventArgs e)
        {
            if (LogFiles.ContainsKey(e.Name))
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var logFileTextBox = (TextBox)this.FindName("log_" + LogFiles[e.Name]);
                        CheckLastLineForCommand(e.FullPath);
                        UpdateLog(logFileTextBox, e.Name, e.FullPath);
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ops! Couldn't read last line in LogFilesOnChanged method: \n" + ex);
                }
            }
        }

        void PrintToGeneralLog(string text)
        {
            log_General.AppendText(text + "\n");
        }

        void CheckLastLineForCommand(string path)
        {
            var lastLine = "";
            try
            {
                lastLine = ReadLines(path).Last();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CheckLastLineForCommand] Ops!\n" + ex.Message);
            }

            if (lastLine.Contains("Connected to Steam"))
            {
                PrintToGeneralLog("Server is running.");
                EnableConsole();
                RCONInitConnect();
                RunUserEvents();
            }
            if (lastLine.Contains("Couldn't Start Server."))
            {
                serverProcess.CloseMainWindow();
                MessageBox.Show("Couldn't Start Server. Ensure that port " +
                    Properties.Settings.Default.Serverport +
                    " is not in use by instance or another application.");
                SwitchWindowLayout(ServerState.STOPPED);
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "taskmgr";
                    p.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ops!\nFailed to open Task manager\nError message:\n" + ex.Message);
                }
            }
        }

        private void UpdateLog(TextBox logFileTextBox, string logFileName, string logFilefullPath)
        {
            try
            {
                int totalLines = ReadLines(logFilefullPath).ToArray().Length;
                int newLinesCount = totalLines - LogFilesReadCount[logFileName];
                var logPart = ReadLines(logFilefullPath).Skip(LogFilesReadCount[logFileName]).Take(newLinesCount).ToArray();
                LogFilesReadCount[logFileName] = totalLines;
                if (logPart.Length > 0)
                {
                    for (int i = 0; i < logPart.Length; i++)
                    {
                        if (!logPart[i].Contains("[Info] [Murkys Core] |||JSON|||") && !string.IsNullOrEmpty(logPart[i]))
                        {
                            logFileTextBox.AppendText(logPart[i] + "\n");
                        }
                        else
                        {
                            ParseCorePluginMessage(logPart[i]);
                        }
                    }
                    logFileTextBox.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UpdateLog] Ops! " + ex.Message + "\n\nPlease report back to author: post@dan-levi.no");
            }
        }

        private void UpdateLog(TextBox logFileTextBox, string logFileName, string logFilefullPath, bool manualUpdate)
        {
            try
            {
                if (File.Exists(Path.Combine(logFilefullPath, logFileName)))
                {
                    logFileTextBox.Clear();
                    var log = File.ReadAllLines(Path.Combine(logFilefullPath, logFileName));
                    if (log.Length > 0)
                    {
                        for (int i = 0; i < log.Length; i++)
                        {
                            if (!log[i].Contains("[Info] [Murkys Core] |||JSON|||") && !string.IsNullOrEmpty(log[i]))
                            {
                                logFileTextBox.AppendText(log[i] + "\n");
                            }
                        }
                        ScrollLogToEnd(logFileTextBox);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ops! Errormessage:\n" + ex.Message + "\n\nPlease report back to author: post@dan-levi.no");
            }
        }

        private static void ScrollLogToEnd(TextBox logFileTextBox)
        {
            logFileTextBox.CaretIndex = logFileTextBox.Text.Length;
            var rect = logFileTextBox.GetRectFromCharacterIndex(logFileTextBox.CaretIndex);
            logFileTextBox.ScrollToHorizontalOffset(rect.Right);
        }

        private TextBox FindLogBox(string name)
        {
            return (TextBox)this.FindName("log_" + LogFiles[name]);
        }




        #endregion

        #region Chat

        enum ChatMode
        {
            Console,
            Private
        };

        Player ChatSessionPlayer;

        ChatMode chatMode = ChatMode.Console;

        void SendChatMessage(Player player, string message)
        {
            if (player != null)
            {
                RCONSendCommand("RustBoot.Whisper " + player.UserID + " \"" + message + "\"", true);
            }
        }

        private void CheckChatSession()
        {
            if (ChatSessionPlayer != null)
            {
                var client = _players.FirstOrDefault(i => i.UserID == ChatSessionPlayer.UserID);
                if (client != null)
                {
                    EnablePlayerActions();
                    chatMode = ChatMode.Private;
                    ConsoleInputMode(ChatMode.Private);
                    Console.WriteLine("Chat is now private with " + ChatSessionPlayer.DisplayName);
                }
            }
        }

        void ConsoleInputMode(ChatMode chatMode)
        {
            if (chatMode == ChatMode.Console)
            {
                txt_Console.BorderBrush = GetColorBrush("Color_InfoBlue");
            }
            else if (chatMode == ChatMode.Private)
            {
                txt_Console.BorderBrush = GetColorBrush("Color_DangerRed");
            }
        }

        #endregion

        #region Server

        bool IsRunning = false;
        bool RustConsoleToggle = false;

        enum ServerState
        {
            RUNNING,
            STOPPED
        };

        void EnableConsole()
        {
            IsRunning = true;
            btn_RestartServer.IsEnabled = true;
            btn_StopServer.IsEnabled = true;
            btn_RustConsole.IsEnabled = true;
        }
        void DisableConsole()
        {
            IsRunning = false;
            btn_RestartServer.IsEnabled = false;
            btn_StopServer.IsEnabled = false;
            btn_RustConsole.IsEnabled = false;
        }

        private void btn_StartServer_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            CheckSettings();
            InitLaunch();
        }

        private void InitLaunch()
        {
            SwitchWindowLayout(ServerState.RUNNING);
            StartServerThread();
        }

        private void CheckForDLLs()
        {
            var newtonsoft = Path.Combine(Environment.CurrentDirectory, "Newtonsoft.Json.dll");
            var queryMaster = Path.Combine(Environment.CurrentDirectory, "QueryMaster.dll");
            var ionicBZip2 = Path.Combine(Environment.CurrentDirectory, "Ionic.BZip2.dll");
            PrintToGeneralLog("Extracting dependencies");

            if (!File.Exists(newtonsoft) || !File.Exists(queryMaster) || !File.Exists(ionicBZip2))
            {
                try
                {
                    CopyEmbeddedResource("DLLs.Newtonsoft.Json.dll", newtonsoft);
                    CopyEmbeddedResource("DLLs.QueryMaster.dll", queryMaster);
                    CopyEmbeddedResource("DLLs.Ionic.BZip2.dll", ionicBZip2);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ops!\n" + ex.Message);
                }
            }
        }

        private void tabs_Server_Running_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                chatMode = ChatMode.Console;
                ConsoleInputMode(ChatMode.Console);
                Console.WriteLine("Chat is now in console mode");
            }
            var tab = (sender as TabControl).SelectedItem as TabItem;
            if (tab.Name != "tab_Players") DisablePlayerActions();
            var logFolder = Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.Rustserverexecutable), "server", Properties.Settings.Default.Identity);
            switch (tab.Name)
            {
                case "tab_Console":
                    UpdateLog(FindLogBox("Log.Log.txt"), "Log.Log.txt", logFolder, true);
                    break;
                case "tab_Chat":
                    UpdateLog(FindLogBox("Log.Chat.txt"), "Log.Chat.txt", logFolder, true);
                    break;
                case "tab_Warning":
                    UpdateLog(FindLogBox("Log.Warning.txt"), "Log.Warning.txt", logFolder, true);
                    break;
                case "tab_Error":
                    UpdateLog(FindLogBox("Log.Error.txt"), "Log.Error.txt", logFolder, true);
                    break;
                case "tab_EAC":
                    UpdateLog(FindLogBox("Log.EAC.txt"), "Log.EAC.txt", logFolder, true);
                    break;
                case "tab_Players":
                    CheckChatSession();
                    break;
            }
        }

        private string GenerateServerArguments()
        {
            var Settings = Properties.Settings.Default;
            string arguments = "";
            arguments += " -batchmode";
            arguments += " -nographics";
            arguments += " +server.port " + Settings.Serverport + " ";
            arguments += " +server.tickrate " + Settings.Tickrate + " ";
            arguments += " +server.maxplayers " + Settings.Maxplayers + " ";
            arguments += " +server.worldsize " + Settings.Worldsize + " ";
            arguments += " +server.saveinterval " + Settings.Saveinterval + " ";
            arguments += " +server.seed " + Settings.Seed + " ";
            if (Settings.RCONenabled)
            {
                arguments += " +rcon.port " + Settings.RCONport + " ";
                arguments += " +rcon.password " + Settings.RCONpassword + " ";
            }
            arguments += " +server.hostname \"" + Settings.Hostname + "\" ";
            if (!string.IsNullOrEmpty(Settings.Identity))
            {
                arguments += " +server.identity \"" + Settings.Identity + "\" ";
            }
            arguments += " +server.description \"" + Settings.Description + "\" ";
            if (!string.IsNullOrEmpty(Settings.Headerimage))
            {
                arguments += " +server.headerimage \"" + Settings.Headerimage + "\" ";
            }
            if (!string.IsNullOrEmpty(Settings.Url))
            {
                arguments += " +server.url \"" + Settings.Url + "\" ";
            }
            if (!string.IsNullOrEmpty(Settings.CustomArguments))
            {
                arguments += Settings.CustomArguments;
            }
            return arguments;
        }

        private void btn_RestartServer_Click(object sender, RoutedEventArgs e)
        {
            RestartServer();
        }

        private void RestartServer(bool force = false)
        {

            if (force != false)
            {
                DoRestart();
            }

            if (MessageBox.Show("Are you sure you want to restart the server?", "Restart server?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DoRestart();
            }
        }

        private void DoRestart()
        {
            DisableConsole();
            txt_Console.IsEnabled = false;
            Console.WriteLine(RCONConnected);
            if (RCONConnected && RCONCheckConnection())
            {
                Console.WriteLine("RCON detected while restarting.");
                RCONSendCommand("quit", true);
                SwitchWindowLayout(ServerState.STOPPED);
                InitLaunch();
            }
            else
            {
                if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
                {
                    serverProcess.CloseMainWindow();
                    InitLaunch();
                    SwitchWindowLayout(ServerState.STOPPED);
                }
            }
        }

        private void btn_StopServer_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (MessageBox.Show("Are you sure you want to stop the server?", "Stop server?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteCorePlugin();
                DisableConsole();
                txt_Console.IsEnabled = false;
                CheckServerData(Properties.Settings.Default);
                if (RCONConnected && RCONCheckConnection())
                {
                    Console.WriteLine("RCON detected while stopping.");
                    RCONSendCommand("quit", true);
                    SwitchWindowLayout(ServerState.STOPPED);
                }
                else
                {
                    if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
                    {
                        serverProcess.CloseMainWindow();
                        SwitchWindowLayout(ServerState.STOPPED);
                    }
                }
            }
        }

        private void btn_RustConsole_Click(object sender, RoutedEventArgs e)
        {
            if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
            {
                if (!RustConsoleToggle)
                {
                    ShowWindow(serverProcess.MainWindowHandle.ToInt32(), SW_SHOW);
                    RustConsoleToggle = !RustConsoleToggle;
                }
                else
                {
                    ShowWindow(serverProcess.MainWindowHandle.ToInt32(), SW_HIDE);
                    RustConsoleToggle = !RustConsoleToggle;
                }

            }
        }

        Process serverProcess;
        private void StartServerThread()
        {
            var serverExecutable = Properties.Settings.Default.Rustserverexecutable;
            try
            {
                serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = Path.GetDirectoryName(serverExecutable),
                        FileName = serverExecutable,
                        Arguments = GenerateServerArguments(),
                        UseShellExecute = false
                    }
                };
                CheckForCorePlugin();
                InitiateLogWatchers();
                InitiatePlayerList();
                PrintToGeneralLog("Initiating server thread.");
                StartProcessWorker();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ops! Errormessage:\n" + ex.Message + "\n\nPlease report back to author: post@dan-levi.no");

            }
        }

        private void StartProcessWorker()
        {
            if (!File.Exists(Properties.Settings.Default.Rustserverexecutable))
            {
                SwitchWindowLayout(ServerState.STOPPED);
                MessageBox.Show("Ops!\nCouldnt locate Rust Server executable. Has the server moved?");
                btn_Rustserverexecutable_Click(btn_Rustserverexecutable as object, new RoutedEventArgs());
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += (sender, e) =>
                {
                    serverProcess.Start();
                    while (serverProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        Thread.Yield();
                    }
                    (sender as BackgroundWorker).ReportProgress(0, "HideProcessWindow");
                };
                worker.ProgressChanged += (o, e) =>
                {
                    if (!string.IsNullOrEmpty(e.UserState as string))
                    {
                        var workerMsg = e.UserState as string;
                        if (workerMsg == "HideProcessWindow")
                        {
                            ShowWindow(serverProcess.MainWindowHandle.ToInt32(), SW_HIDE);
                        }
                    }
                };
                worker.RunWorkerAsync();
            }

        }

        #endregion

        #region RCON

        Server RCONRustServer;
        ServerInfo RCONRustServerInfo;
        bool RCONConnected = false;

        void RCONInitConnect()
        {
            var Settings = Properties.Settings.Default;
            if (!Settings.RCONenabled)
            {
                txt_Console.IsEnabled = false;
                MessageBox.Show("RCON is not enabled.\nTwo way communication services are disabled.");
            }
            else
            {
                var ip = "127.0.0.1";
                var port = Settings.RCONport;
                var pass = Settings.RCONpassword;
                if (RCONConnect(ip, pass, port))
                {
                    txt_Console.IsEnabled = true;
                }
                else
                {
                    txt_Console.IsEnabled = false;
                    MessageBox.Show("Error while trying to establish RCON connection");
                }
            }
        }

        bool RCONConnect(string ip, string pass, int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            RCONRustServer = ServerQuery.GetServerInstance(EngineType.Source, ep);
            RCONRustServerInfo = RCONRustServer.GetInfo();
            if (RCONRustServer.GetControl(pass))
            {
                RCONConnected = true;
                return true;
            }
            RCONConnected = false;
            return false;
        }

        bool RCONCheckConnection()
        {
            if (RCONRustServer != null && RCONRustServer.GetControl(Properties.Settings.Default.RCONpassword))
            {
                return true;
            }
            return false;
        }

        void RCONDispose()
        {
            RCONRustServer.Dispose();
            RCONRustServer = null;
            RCONRustServerInfo = null;
        }

        private void txt_Console_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (chatMode == ChatMode.Private)
                {
                    if (ChatSessionPlayer != null)
                    {
                        SendChatMessage(ChatSessionPlayer, txt_Console.Text);
                        txt_Console.Text = "";
                    }
                }
                else
                {
                    RCONSendCommand(txt_Console.Text);
                    txt_Console.Text = "";
                }
            }
        }

        static string[] RCONCriticalCommands = {
            "quit",
            "server.stop",
            "stop",
            "server.restart",
            "restart"
        };

        void RCONSendCommand(string input, bool force = false)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (force)
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        RCONRustServer.Rcon.SendCommand(input);
                    }).Start();
                }
                else
                {
                    var command = RCONCriticalCommands.FirstOrDefault(n => input.StartsWith(n));
                    if (command != null)
                    {
                        if (command == "quit" || command == "server.stop" || command == "stop")
                            StopServer();
                        if (command == "server.restart")
                            RestartServer();
                        if (command.StartsWith("restart"))
                            RestartServerDelayed(input);
                    }
                    else
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            RCONRustServer.Rcon.SendCommand(input);
                        }).Start();
                    }
                }
            }
        }

        private void RestartServerDelayed(string command)
        {
            Console.WriteLine(command);
            var split = command.Split(null);
            if (split.Length == 1)
            {
                RestartServer();
            }
            else
            {
                int _timeout;
                int.TryParse(split[1], out _timeout);
                if (_timeout > 0)
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        RCONSendCommand("say Restart in " + _timeout.ToString() + " seconds.", true);
                        Thread.Sleep(TimeSpan.FromSeconds(_timeout / 2));
                        RCONSendCommand("say Restart in " + (_timeout / 2).ToString() + " seconds.", true);
                        Thread.Sleep(TimeSpan.FromSeconds(_timeout / 2));
                        RCONSendCommand("say Restarting...", true);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            RestartServer(true);
                        }));
                    }).Start();
                }
            }
        }

        #endregion

        #region Tabs

        private void tabs_Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var selectedTab = (sender as TabControl).SelectedItem as TabItem;

                if (selectedTab.Header.ToString() == "Plugins")
                {
                    btn_DeletePlugins.Visibility = Visibility.Visible;
                    stack_Btns_Settings_Lower.Margin = new Thickness(0, 0, 0, -40);
                    if (e.Source is TabControl)
                    {
                        e.Handled = true;
                        RefreshPluginLists();
                    }
                }
                else
                {
                    btn_DeletePlugins.Visibility = Visibility.Collapsed;
                    stack_Btns_Settings_Lower.Margin = new Thickness(0, 0, 0, 0);
                }
                if (selectedTab.Header.ToString() == "Events")
                {
                    btn_StartServer.IsEnabled = false;
                }
                else
                {
                    btn_StartServer.IsEnabled = true;
                }
            }
        }


        #endregion

        #region Helpers

        string Randomstring(int length)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                .Select(s => s[new Random()
                .Next(s.Length)])
                .ToArray());
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Copies embedded resource to given path
        /// </summary>
        /// <param name="source">example: Plugins.MurkysCore.cs</param>
        /// <param name="destination">Full path with filename</param>
        void CopyEmbeddedResource(string source, string destination)
        {
            using (FileStream fileStream = File.Create(destination))
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream("MurkysRustBoot." + source).CopyTo(fileStream);
            }
        }

        public static SolidColorBrush GetColorBrush(string name)
        {
            return Application.Current.Resources[name] as SolidColorBrush;
        }

        #endregion

        #region Misc

        private void img_RustLogo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://playrust.com/");
        }

        private void img_OxideLogo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("http://oxidemod.org/");
        }











        #endregion

        
    }

    public static class ExtensionMethods
    {
        public static int ToInt(this string value)
        {
            int _int;
            Int32.TryParse(value, out _int);
            return _int;
        }
        public static string Sanitize(this string value)
        {
            value = value.ToLower().Trim().Replace(" ", "_");
            return Regex.Replace(value, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}
