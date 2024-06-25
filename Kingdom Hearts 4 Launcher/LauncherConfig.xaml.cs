using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kingdom_Hearts_4_Launcher
{
    public partial class LauncherConfig : Window
    {
        public List<string> GameOrder { get; private set; }
        public string SelectedOrder { get; private set; }
        public bool SkipCopyrightScreenOnMovies { get; private set; }
        public bool SkipCopyrightScreenOnKH1 { get; private set; }
        public string MelonMixPath { get; private set; }
        public bool UseMelonMixOnDays { get; private set; }
        public bool UseMelonMixOnRecoded { get; private set; }
        public string EmulatorPath { get; private set; }
        public string RomPath { get; private set; }
        public bool ComInsteadOfRecom { get; private set; }

        private bool romexists = false;
        private bool emuexists = false;

        public LauncherConfig(string selectedOrder, bool skipCopyrightScreenOnMovies, bool skipCopyrightScreenOnKH1, string melonMixPath, bool useMelonMixOnDays, bool useMelonMixOnRecoded, string emulatorPath, string romPath, bool comInsteadOfRecom)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(melonMixPath) && File.Exists(melonMixPath))
            {
                melonmixdirectory.Text = melonMixPath;
                UseMelonMixOnDaysCheckBox.IsEnabled = true;
                UseMelonMixOnRecodedCheckBox.IsEnabled = true;
            }
            else
            {
                melonmixdirectory.Text = null;
                UseMelonMixOnDaysCheckBox.IsEnabled = false;
                UseMelonMixOnRecodedCheckBox.IsEnabled = false;
                useMelonMixOnDays = false;
                useMelonMixOnRecoded = false;
            }

            if (!string.IsNullOrEmpty(emulatorPath) && File.Exists(emulatorPath))
            {
                emulatordirectory.Text = emulatorPath;
                emuexists = true;
            }
            else
            {
                emulatordirectory.Text = null;
            }

            if (!string.IsNullOrEmpty(romPath) && File.Exists(romPath))
            {
                romdirectory.Text = romPath;
                romexists = true;
            }
            else
            {
                romdirectory.Text = null;
            }

            if (emuexists && romexists)
            {
                ComInsteadOfRecomCheckBox.IsEnabled = true;
            }
            else
            {
                comInsteadOfRecom = false;
            }

            SelectedOrder = selectedOrder;
            SkipCopyrightScreenOnMoviesCheckBox.IsChecked = skipCopyrightScreenOnMovies;
            SkipCopyrightScreenOnKH1CheckBox.IsChecked = skipCopyrightScreenOnKH1;
            UseMelonMixOnDaysCheckBox.IsChecked = useMelonMixOnDays;
            UseMelonMixOnRecodedCheckBox.IsChecked = useMelonMixOnRecoded;
            ComInsteadOfRecomCheckBox.IsChecked = comInsteadOfRecom;

            foreach (ComboBoxItem item in PresetsComboBox.Items)
            {
                if (item.Content.ToString() == SelectedOrder)
                {
                    PresetsComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            while (true)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "All files (*.*)|*.*"
                    };

                if (openFileDialog.ShowDialog() == true)
                {
                    if (Path.GetFileName(openFileDialog.FileName).Contains("khDaysMM") || Path.GetFileName(openFileDialog.FileName).Contains("MelonMix"))
                    {
                        melonmixdirectory.Text = openFileDialog.FileName;
                        UseMelonMixOnDaysCheckBox.IsEnabled = true;
                        UseMelonMixOnRecodedCheckBox.IsEnabled = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Please select the khDaysMM or MelonMix executable.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void Browseemu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                MessageBox.Show("Keep in mind this application does not verify whether what you selected is a valid GBA emulator.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                emulatordirectory.Text = openFileDialog.FileName;
                emuexists = true;
                if (emuexists && romexists)
                {
                    ComInsteadOfRecomCheckBox.IsEnabled = true;
                }
            }
        }

        private void Browserom_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                MessageBox.Show("Keep in mind this application does not verify whether what you selected is a valid Chain of Memories ROM.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                romdirectory.Text = openFileDialog.FileName;
                romexists = true;
                if (emuexists && romexists)
                {
                    ComInsteadOfRecomCheckBox.IsEnabled = true;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedOrder = (PresetsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            SkipCopyrightScreenOnMovies = SkipCopyrightScreenOnMoviesCheckBox.IsChecked ?? false;
            SkipCopyrightScreenOnKH1 = SkipCopyrightScreenOnKH1CheckBox.IsChecked ?? false;
            MelonMixPath = melonmixdirectory.Text;
            UseMelonMixOnDays = UseMelonMixOnDaysCheckBox.IsChecked ?? false;
            UseMelonMixOnRecoded = UseMelonMixOnRecodedCheckBox.IsChecked ?? false;
            EmulatorPath = emulatordirectory.Text;
            RomPath = romdirectory.Text;
            ComInsteadOfRecom = ComInsteadOfRecomCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PresetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PresetsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedPreset = selectedItem.Content.ToString();
                switch (selectedPreset)
                {
                    case "Official":
                        GameOrder = new List<string> { "kh1", "recom", "days", "kh2", "bbs", "recoded" };
                        break;
                    case "Release":
                        GameOrder = new List<string> { "kh1", "recom", "kh2", "days", "bbs", "recoded" };
                        break;
                    case "Chronological":
                        GameOrder = new List<string> { "bbs", "kh1", "recom", "days", "kh2", "recoded" };
                        break;
                    case "Alphabetical":
                        GameOrder = new List<string> { "bbs", "kh1", "recom", "recoded", "days", "kh2" };
                        break;
                    case "Alphabetical 2":
                        GameOrder = new List<string> { "bbs", "kh1", "kh2", "recom", "recoded", "days" };
                        break;
                    case "Official reverse":
                        GameOrder = new List<string> { "recoded", "bbs", "kh2", "days", "recom", "kh1" };
                        break;
                    case "Release reverse":
                        GameOrder = new List<string> { "recoded", "bbs", "days", "kh2", "recom", "kh1" };
                        break;
                    case "Chronological reverse":
                        GameOrder = new List<string> { "recoded", "kh2", "days", "recom", "kh1", "bbs" };
                        break;
                    case "Alphabetical reverse":
                        GameOrder = new List<string> { "kh2", "days", "recoded", "recom", "kh1", "bbs" };
                        break;
                    case "Alphabetical 2 reverse":
                        GameOrder = new List<string> { "days", "recoded", "recom", "kh2", "kh1", "bbs" };
                        break;
                    case "Random":
                        List<string> defaultOrder = new List<string> { "kh1", "recom", "days", "kh2", "bbs", "recoded" };
                        GameOrder = ShuffleList(defaultOrder);
                        break;
                }
            }
        }

        private List<string> ShuffleList(List<string> list)
        {
            Random rng = new Random();
            return list.OrderBy(item => rng.Next()).ToList();
        }
    }
}
