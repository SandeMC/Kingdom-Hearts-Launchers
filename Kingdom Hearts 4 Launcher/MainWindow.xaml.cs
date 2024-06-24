using System;
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
        public string MelonMixPath { get; private set; }
        public bool UseMelonMixOnDays { get; private set; }
        public bool UseMelonMixOnRecoded { get; private set; }
        public string SelectedOrder { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadGameOrder();
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

        private void SaveGameOrder()
        {
            var order = Games.Children.OfType<RadioButton>().Select(rb => rb.Name).ToList();
            var CheckedGame = Games.Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true)?.Name;

            var config = new GameOrderConfig
            {
                Order = order,
                CheckedGame = CheckedGame,
                MelonMixPath = MelonMixPath,
                UseMelonMixOnDays = UseMelonMixOnDays,
                UseMelonMixOnRecoded = UseMelonMixOnRecoded,
                SelectedOrder = SelectedOrder
            };

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        private void LoadGameOrder()
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
                    SaveGameOrder();
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

                MelonMixPath = config.MelonMixPath;
                UseMelonMixOnDays = config.UseMelonMixOnDays;
                UseMelonMixOnRecoded = config.UseMelonMixOnRecoded;
                SelectedOrder = config.SelectedOrder;
            }
            else
            {
                SaveGameOrder();
            }
        }

        private class GameOrderConfig
        {
            public List<string> Order { get; set; }
            public string CheckedGame { get; set; }
            public string MelonMixPath { get; set; }
            public bool UseMelonMixOnDays { get; set; }
            public bool UseMelonMixOnRecoded { get; set; }
            public string SelectedOrder { get; set; }
        }

        private void LauncherConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new LauncherConfig(MelonMixPath, UseMelonMixOnDays, UseMelonMixOnRecoded, SelectedOrder);
            if (configWindow.ShowDialog() == true)
            {
                MelonMixPath = configWindow.MelonMixPath;
                UseMelonMixOnDays = configWindow.UseMelonMixOnDays;
                UseMelonMixOnRecoded = configWindow.UseMelonMixOnRecoded;
                SelectedOrder = configWindow.SelectedOrder;

                if (configWindow.GameOrder != null && configWindow.GameOrder.Count > 0)
                {
                    ReorderGames(configWindow.GameOrder.ToArray());
                }

                SaveGameOrder();
            }
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            // melon mix stuff
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = MelonMixPath;
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(MelonMixPath);

            if (kh1.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS FINAL MIX.exe");
            }
            if (recom.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS Re_Chain of Memories.exe");
            }
            if (days.IsChecked == true)
            {
                if (UseMelonMixOnDays)
                {
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    Close();
                }
                else
                {
                    LaunchGame("KINGDOM HEARTS HD 1.5+2.5 Launcher.exe");
                }
            }
            if (kh2.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS II FINAL MIX.exe");
            }
            if (bbs.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS Birth by Sleep FINAL MIX.exe");
            }
            if (recoded.IsChecked == true)
            {
                if (UseMelonMixOnRecoded)
                {
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    Close();
                }
                else
                {
                    LaunchGame("KINGDOM HEARTS HD 1.5+2.5 Launcher.exe");
                }
            }
        }

        private void LaunchGame(string exeName)
        {
            if (File.Exists(exeName))
            {
                Process.Start(exeName);
                Close();
            }
            else
            {
                MessageBox.Show("Selected game's executable not found");
            }
        }
    }
}
