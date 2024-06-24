using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kingdom_Hearts_2_Launcher
{
    public partial class LauncherConfig : Window
    {
        public List<string> GameOrder { get; private set; }
        public string SelectedOrder { get; private set; }

        public LauncherConfig(string selectedOrder)
        {
            InitializeComponent();
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
                        GameOrder = new List<string> { "khddd", "bbs02", "xbackcover" };
                        break;
                    case "Chronological":
                        GameOrder = new List<string> { "xbackcover", "bbs02", "khddd" };
                        break;
                    case "Alphabetical":
                        GameOrder = new List<string> { "xbackcover", "khddd", "bbs02" };
                        break;
                    case "Alphabetical reverse":
                        GameOrder = new List<string> { "bbs02", "khddd", "xbackcover" };
                        break;
                    case "Random":
                        List<string> defaultOrder = new List<string> { "khddd", "bbs02", "xbackcover" };
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
