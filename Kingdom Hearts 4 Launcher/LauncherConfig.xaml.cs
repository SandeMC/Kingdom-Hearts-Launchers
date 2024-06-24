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
        public string MelonMixPath { get; private set; }
        public bool UseMelonMixOnDays { get; private set; }
        public bool UseMelonMixOnRecoded { get; private set; }
        public List<string> GameOrder { get; private set; }
        public string SelectedOrder { get; private set; }

        public LauncherConfig(string melonMixPath, bool useMelonMixOnDays, bool useMelonMixOnRecoded, string selectedOrder)
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
            }

            UseMelonMixOnDaysCheckBox.IsChecked = useMelonMixOnDays;
            UseMelonMixOnRecodedCheckBox.IsChecked = useMelonMixOnRecoded;
            SelectedOrder = selectedOrder;

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
                        Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
                    };

                if (openFileDialog.ShowDialog() == true)
                {
                    if (Path.GetFileName(openFileDialog.FileName) == "khDaysMM.exe")
                    {
                        melonmixdirectory.Text = openFileDialog.FileName;
                        UseMelonMixOnDaysCheckBox.IsEnabled = true;
                        UseMelonMixOnRecodedCheckBox.IsEnabled = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Please select the khDaysMM.exe file.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    break;
                }
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MelonMixPath = melonmixdirectory.Text;
            UseMelonMixOnDays = UseMelonMixOnDaysCheckBox.IsChecked ?? false;
            UseMelonMixOnRecoded = UseMelonMixOnRecodedCheckBox.IsChecked ?? false;
            SelectedOrder = (PresetsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

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
                    case "Default":
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
                    case "Default reverse":
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
