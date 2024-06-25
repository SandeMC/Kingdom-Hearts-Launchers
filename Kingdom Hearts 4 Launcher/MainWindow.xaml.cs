using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Kingdom_Hearts_4_Launcher
{
    public partial class MainWindow : Window
    {
        private readonly string ConfigFilePath = "launcher_config.json";
        public string SelectedOrder { get; private set; }
        public bool SkipCopyrightScreenOnMovies { get; private set; }
        public bool SkipCopyrightScreenOnKH1 { get; private set; }
        public string MelonMixPath { get; private set; }
        public bool UseMelonMixOnDays { get; private set; }
        public bool UseMelonMixOnRecoded { get; private set; }
        public string EmulatorPath { get; private set; }
        public string RomPath { get; private set; }
        public bool ComInsteadOfRecom { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
            if (!File.Exists("steam_appid.txt"))
            {
                File.WriteAllText("steam_appid.txt", "2552430");
            }
        }

        private void ReorderGames(string[] newOrder)
        {
            var radioButtons = new Dictionary<string, RadioButton>
        {
            { kh1.Name, kh1 },
            { recom.Name, recom },
            { days.Name, days },
            { kh2.Name, kh2 },
            { bbs.Name, bbs },
            { recoded.Name, recoded }
        };

            Games.Children.Clear();
            foreach (var name in newOrder)
            {
                if (radioButtons.ContainsKey(name))
                {
                    Games.Children.Add(radioButtons[name]);
                }
            }
        }

        private void SaveConfig()
        {
            var order = Games.Children.OfType<RadioButton>().Select(rb => rb.Name).ToList();
            var CheckedGame = Games.Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true)?.Name;

            var config = new GameOrderConfig
            {
                Order = order,
                SelectedOrder = SelectedOrder,
                CheckedGame = CheckedGame,
                SkipCopyrightScreenOnMovies = SkipCopyrightScreenOnMovies,
                SkipCopyrightScreenOnKH1 = SkipCopyrightScreenOnKH1,
                MelonMixPath = MelonMixPath,
                UseMelonMixOnDays = UseMelonMixOnDays,
                UseMelonMixOnRecoded = UseMelonMixOnRecoded,
                EmulatorPath = EmulatorPath,
                RomPath = RomPath,
                ComInsteadOfRecom = ComInsteadOfRecom
            };

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        private void LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                GameOrderConfig config;
                try
                {
                    config = JsonConvert.DeserializeObject<GameOrderConfig>(json);
                }
                catch
                {
                    MessageBox.Show("Unable to read config file, settings will be reset.");
                    SaveConfig();
                    return;
                }

                if (config.Order != null && config.Order.Count > 0)
                {
                    ReorderGames(config.Order.ToArray());

                    if (!string.IsNullOrEmpty(config.CheckedGame))
                    {
                        var checkedRadioButton = Games.Children.OfType<RadioButton>().FirstOrDefault(rb => rb.Name == config.CheckedGame);
                        if (checkedRadioButton != null)
                        {
                            checkedRadioButton.IsChecked = true;
                        }
                    }
                }
                SelectedOrder = config.SelectedOrder;
                SkipCopyrightScreenOnMovies = config.SkipCopyrightScreenOnMovies;
                SkipCopyrightScreenOnKH1 = config.SkipCopyrightScreenOnKH1;

                bool emuexists = !string.IsNullOrEmpty(config.EmulatorPath) && File.Exists(config.EmulatorPath);
                bool romexists = !string.IsNullOrEmpty(config.RomPath) && File.Exists(config.RomPath);

                MelonMixPath = !string.IsNullOrEmpty(config.MelonMixPath) && File.Exists(config.MelonMixPath)
                                ? config.MelonMixPath
                                : null;
                UseMelonMixOnDays = MelonMixPath != null && config.UseMelonMixOnDays;
                UseMelonMixOnRecoded = MelonMixPath != null && config.UseMelonMixOnRecoded;

                EmulatorPath = emuexists ? config.EmulatorPath : null;
                RomPath = romexists ? config.RomPath : null;

                ComInsteadOfRecom = emuexists && romexists && config.ComInsteadOfRecom;

                days.Content = UseMelonMixOnDays
                                ? "Kingdom Hearts 358/2 Days (Melon Mix)"
                                : "Kingdom Hearts 358/2 Days (Movie)";
                recoded.Content = UseMelonMixOnRecoded
                                ? "Kingdom Hearts Re:coded (Melon Mix)"
                                : "Kingdom Hearts Re:coded (Movie)";
                recom.Content = ComInsteadOfRecom
                                ? "Kingdom Hearts: Chain of Memories (GBA)"
                                : "Kingdom Hearts Re:Chain of Memories";
            }
            else
            {
                SkipCopyrightScreenOnMovies = true;
                SkipCopyrightScreenOnKH1 = true;
                SaveConfig();
            }
        }

        private class GameOrderConfig
        {
            public List<string> Order { get; set; }
            public string SelectedOrder { get; set; }
            public string CheckedGame { get; set; }
            public bool SkipCopyrightScreenOnMovies { get; set; }
            public bool SkipCopyrightScreenOnKH1 { get; set; }
            public string MelonMixPath { get; set; }
            public bool UseMelonMixOnDays { get; set; }
            public bool UseMelonMixOnRecoded { get; set; }
            public string EmulatorPath { get; set; }
            public string RomPath { get; set; }
            public bool ComInsteadOfRecom { get; set; }
        }

        private void LauncherConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new LauncherConfig(SelectedOrder, SkipCopyrightScreenOnMovies, SkipCopyrightScreenOnKH1, MelonMixPath, UseMelonMixOnDays, UseMelonMixOnRecoded, EmulatorPath, RomPath, ComInsteadOfRecom);
            if (configWindow.ShowDialog() == true)
            {
                SelectedOrder = configWindow.SelectedOrder;
                SkipCopyrightScreenOnMovies = configWindow.SkipCopyrightScreenOnMovies;
                SkipCopyrightScreenOnKH1 = configWindow.SkipCopyrightScreenOnKH1;
                MelonMixPath = configWindow.MelonMixPath;
                UseMelonMixOnDays = configWindow.UseMelonMixOnDays;
                UseMelonMixOnRecoded = configWindow.UseMelonMixOnRecoded;
                EmulatorPath = configWindow.EmulatorPath;
                RomPath = configWindow.RomPath;
                ComInsteadOfRecom = configWindow.ComInsteadOfRecom;

                if (configWindow.GameOrder != null && configWindow.GameOrder.Count > 0)
                {
                    ReorderGames(configWindow.GameOrder.ToArray());
                }

                if (UseMelonMixOnDays)
                {
                    days.Content = "Kingdom Hearts 358/2 Days (Melon Mix)";
                }
                else
                {
                    days.Content = "Kingdom Hearts 358/2 Days (Movie)";
                }

                if (UseMelonMixOnRecoded)
                {
                    recoded.Content = "Kingdom Hearts Re:coded (Melon Mix)";
                }
                else
                {
                    recoded.Content = "Kingdom Hearts Re:coded (Movie)";
                }

                if (ComInsteadOfRecom)
                {
                    recom.Content = "Kingdom Hearts: Chain of Memories (GBA)";
                }
                else
                {
                    recom.Content = "Kingdom Hearts Re:Chain of Memories";
                }

                SaveConfig();
            }
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            if (kh1.IsChecked == true)
            {
                string arg = "";
                if (SkipCopyrightScreenOnKH1)
                {
                    arg = "-reboot=true";
                }
                LaunchGame("KINGDOM HEARTS FINAL MIX.exe", arg);
            }
            else if (recom.IsChecked == true)
            {
                if (ComInsteadOfRecom)
                {
                    LaunchGame(EmulatorPath, $"\"{RomPath}\"");
                }
                else
                {
                    LaunchGame("KINGDOM HEARTS Re_Chain of Memories.exe");
                }
            }
            else if (days.IsChecked == true)
            {
                if (UseMelonMixOnDays)
                {
                    LaunchGame(MelonMixPath);
                }
                else
                {
                    string arg = "";
                    if (SkipCopyrightScreenOnMovies)
                    {
                        arg = "-reboot=true";
                    }
                    LaunchGame("KINGDOM HEARTS HD 1.5+2.5 Launcher.exe", arg);
                }
            }
            else if (kh2.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS II FINAL MIX.exe");
            }
            else if (bbs.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS Birth by Sleep FINAL MIX.exe");
            }
            else if (recoded.IsChecked == true)
            {
                if (UseMelonMixOnRecoded)
                {
                    Close();
                }
                else
                {
                    string arg = "";
                    if (SkipCopyrightScreenOnMovies)
                    {
                        arg = "-reboot=true";
                    }
                    LaunchGame("KINGDOM HEARTS HD 1.5+2.5 Launcher.exe", arg);
                }
            }
        }

        private void LaunchGame(string exeName, string arguments = "")
        {
            if (File.Exists(exeName))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exeName,
                    WorkingDirectory = Path.GetDirectoryName(exeName),
                    Arguments = arguments
                };
                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();
                SaveConfig();
                Close();
            }
            else
            {
                MessageBox.Show("Selected game's executable not found");
            }
        }
    }
}
