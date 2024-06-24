using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Kingdom_Hearts_2_Launcher
{
    public partial class MainWindow : Window
    {
        private readonly string ConfigFilePath = "launcher_config.json";
        public string SelectedOrder { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadGameOrder();
            if (!File.Exists("steam_appid.txt"))
            {
                File.WriteAllText("steam_appid.txt", "2552440");
            }
            if (!File.Exists("KINGDOM HEARTS 0.2 Birth by Sleep/Binaries/Win64/steam_appid.txt"))
            {
                File.WriteAllText("KINGDOM HEARTS 0.2 Birth by Sleep/Binaries/Win64/steam_appid.txt", "2552440");
            }
        }

        private void ReorderGames(string[] newOrder)
        {
            var radioButtons = new Dictionary<string, RadioButton>
        {
            { khddd.Name, khddd },
            { bbs02.Name, bbs02 },
            { xbackcover.Name, xbackcover }
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
            public string SelectedOrder { get; set; }
        }

        private void LauncherConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new LauncherConfig(SelectedOrder);
            if (configWindow.ShowDialog() == true)
            {
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

            if (khddd.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS FINAL MIX.exe");
            }
            if (bbs02.IsChecked == true)
            {
                if (File.Exists("KINGDOM HEARTS 0.2 Birth by Sleep/Binaries/Win64/KINGDOM HEARTS 0.2 Birth by Sleep.exe"))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "KINGDOM HEARTS 0.2 Birth by Sleep/Binaries/Win64/KINGDOM HEARTS 0.2 Birth by Sleep.exe";
                    startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName("KINGDOM HEARTS 0.2 Birth by Sleep/Binaries/Win64/KINGDOM HEARTS 0.2 Birth by Sleep.exe");
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    Close();
                }
                else
                {
                    MessageBox.Show("Selected game's executable not found");
                }
            }
            if (xbackcover.IsChecked == true)
            {
                LaunchGame("KINGDOM HEARTS HD 2.8 Launcher.exe");
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
