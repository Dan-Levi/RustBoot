using Microsoft.Win32;
using MurkysRustBoot.Classes;
using MurkysRustBoot.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            DataContext = this;
            Window_Settings.Visibility = Visibility.Visible;
            Window_Running.Visibility = Visibility.Collapsed;
            DeleteCorePlugin();
            LoadSettings();
            CheckSettings();
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
                if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
                {
                    serverProcess.CloseMainWindow();
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
            var serverPath = Path.Combine(Path.GetDirectoryName(Settings.Rustserverexecutable), "server", Settings.Identity);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            Settings.CustomArguments = txt_Customarguments.Text;
            Settings.Save();
        }

        void DefaultSettings()
        {
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
                tab_Plugins.IsEnabled = true;
                if (!string.IsNullOrEmpty(Settings.Hostname) && Settings.Maxplayers > 0 && Settings.Worldsize > 0 && Settings.Seed.ToString().Length == 5)
                {
                    if (Settings.RCONenabled)
                    {
                        if (Settings.RCONport == 0 || string.IsNullOrEmpty(Settings.RCONpassword))
                        {
                            MessageBox.Show("RCON port and passord must be specified.");
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
                    MessageBox.Show("Error detected in General tab. Revert to defaults if in doubt.");
                    btn_StartServer.IsEnabled = false;
                }
            }
            else
            {
                tab_Plugins.IsEnabled = false;
                btn_StartServer.IsEnabled = false;
                btn_Rustserverexecutable_Click(btn_StartServer as object, new RoutedEventArgs());
            }
        }




        #endregion

        #region GUI

        private void btn_Seed_Click(object sender, RoutedEventArgs e)
        {
            txt_Seed.Text = (new Random()).Next(00000, 99999).ToString("D5");
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
                    break;
                case ServerState.STOPPED:
                    Window_Settings.Visibility = Visibility.Visible;
                    btn_Rustserverexecutable.IsEnabled = true;
                    btn_SaveSettings.IsEnabled = true;
                    btn_ResetSettings.IsEnabled = true;
                    lbl_Serverexecutable.Visibility = Visibility.Visible;
                    txt_Rustserverexecutable.Visibility = Visibility.Visible;
                    Window_Running.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
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

        #endregion

        #region Plugins

        string PluginDirectory;
        string DeactivatedPluginsDirectory;
        List<string> Plugins;
        List<string> DeactivatedPlugins;
        List<string> ValidPluginExtensions = new List<string> {
            ".cs",".lua"
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
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Identity))
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

        string[] KnownServerMessages = {
            "Couldn't Start Server.",
            "Connected to Steam"
        };

        void CheckLastLineForCommand(string path)
        {
            var lastLine = ReadLines(path).Last();
            if (lastLine.Contains("Connected to Steam"))
                EnableConsole();
            if (lastLine.Contains("Couldn't Start Server."))
            {
                serverProcess.CloseMainWindow();
                MessageBox.Show("Couldn't Start Server. Ensure that port " +
                    Properties.Settings.Default.Serverport +
                    "is not in use by another instance of the server or another application.");
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
            catch (Exception ex)
            {
                MessageBox.Show("Ops! " + ex.Message + "\n\nPlease report back to author: post@dan-levi.no");
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
            catch (Exception ex)
            {
                MessageBox.Show("Ops! Errormessage:\n" + ex.Message + "\n\nPlease report back to author: post@dan-levi.no");
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
            CheckForNewtonsoftJSON();
            StartServerThread();
        }

        private void CheckForNewtonsoftJSON()
        {
            var destinationfolder = Path.Combine(Environment.CurrentDirectory, "Newtonsoft.Json.dll");
            if (!File.Exists(destinationfolder))
            {
                CopyEmbeddedResource("DLLs.Newtonsoft.Json.dll", destinationfolder);
            }
        }

        private void tabs_Server_Running_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (sender as TabControl).SelectedItem as TabItem;
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
                default:
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
            Console.WriteLine(arguments);
            return arguments;
        }

        private void btn_RestartServer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart the server?", "Restart server?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DisableConsole();
                SwitchWindowLayout(ServerState.STOPPED);
                if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
                {
                    serverProcess.CloseMainWindow();
                    InitLaunch();
                }
            }
        }

        private void btn_StopServer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to stop the server?", "Stop server?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteCorePlugin();
                DisableConsole();
                if (serverProcess != null && serverProcess.MainWindowHandle != IntPtr.Zero)
                {
                    serverProcess.CloseMainWindow();
                    SwitchWindowLayout(ServerState.STOPPED);
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
                StartProcessWorker();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ops! Errormessage:\n" + ex.Message + "\n\nPlease report back to author: post@dan-levi.no");

            }
        }

        private void StartProcessWorker()
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

        #endregion
        
        #region Tabs

        private void tabs_Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }
        
        #endregion

        #region Helpers

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

        SolidColorBrush GetColorBrush(string name)
        {
            return Resources[name] as SolidColorBrush;
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
