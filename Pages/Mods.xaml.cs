using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;

namespace Assassins_Creed_Remastered_Launcher.Pages
{
    /// <summary>
    /// Interaction logic for Mods.xaml
    /// </summary>
    public partial class Mods : Page
    {
        // Global Vars
        private ObservableCollection<string> EnabledMods = new ObservableCollection<string> { };
        private ObservableCollection<string> DisabledMods = new ObservableCollection<string> { };
        private Dictionary<string, string> InstalledMods = new Dictionary<string, string>();
        private Dictionary<string, string> InstalledEnabledMods = new Dictionary<string, string>();
        private Dictionary<string, string> InstalledDisabledMods = new Dictionary<string, string>();
        private bool isSelectionEnabledModsChangingProgrammatically = false;
        private bool isSelectionDisabledModsChangingProgrammatically = false;

        public Mods()
        {
            InitializeComponent();
            EnabledModsList.ItemsSource = EnabledMods;
            DisabledModsList.ItemsSource = DisabledMods;
            ReaduModConfig();
        }

        // Grabs all of the uMod mods inside of Mods folder and reads uMod configuration file
        private void ReaduModConfig()
        {
            try
            {
                // First grabs all folders inside of Mods then reads all of the files inside those folders
                string[] directories = Directory.GetDirectories(App.path + @"\Mods\");
                foreach (string dir in directories)
                {
                    string[] mods = Directory.GetFiles(dir);
                    foreach (string mod in mods)
                    {
                        InstalledMods.Add(System.IO.Path.GetFileName(mod), mod);
                    }
                }
                // Check to see if template even exists and mods that are already enabled are put into Dictionary EnabledMods
                if (System.IO.File.Exists(App.path + @"\uMod\templates\ac1.txt"))
                {
                    string[] uModConfig = File.ReadAllLines(App.path + @"\uMod\templates\ac1.txt");
                    foreach (string line in uModConfig)
                    {
                        if (line.StartsWith("Add_true:"))
                        {
                            string path = line.Substring("Add_true:".Length);
                            if (InstalledMods.ContainsValue(path))
                            {
                                EnabledMods.Add(System.IO.Path.GetFileName(path));
                                InstalledEnabledMods.Add(System.IO.Path.GetFileName(path), path);
                                InstalledMods.Remove(System.IO.Path.GetFileName(path));
                            }
                        }
                    }
                }
                // Now I put every mod that is not enabled into Dictionary DisabledMods
                foreach (string mod in InstalledMods.Keys)
                {
                    DisabledMods.Add(mod);
                    InstalledDisabledMods.Add(mod, InstalledMods[mod]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(App.path + @"\uMod\templates\ac1.txt"))
                {
                    using (StreamWriter sw = new StreamWriter(App.path + @"\uMod\templates\ac1.txt"))
                    {
                        sw.Write("SaveAllTextures:0\n");
                        sw.Write("SaveSingleTexture:0\n");
                        sw.Write("FontColour:255,0,0\n");
                        sw.Write("TextureColour:0,255,0\n");
                        foreach (string mod in EnabledMods)
                        {
                            sw.Write("Add_true:" + InstalledEnabledMods[mod] + "\n");
                        }
                    }
                }
                MessageBox.Show("Saving done.");
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async void AddMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog FileDialog = new OpenFileDialog();
                FileDialog.Filter = "uMod File|*.tpf";
                FileDialog.Title = "Select .tpf uMod Mod for Assassin's Creed";
                string modPath;
                if (FileDialog.ShowDialog() == true)
                {
                    modPath = FileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Installation Cancelled");
                    return;
                }
                if (System.IO.File.Exists(modPath))
                {
                    if (!System.IO.File.Exists(App.path + $@"\Mods\Custom uMods\{System.IO.Path.GetFileName(modPath)}"))
                    {
                        System.IO.File.Move(modPath, App.path + $@"\Mods\Custom uMods\{System.IO.Path.GetFileName(modPath)}");
                        EnabledMods.Add(System.IO.Path.GetFileName(modPath));
                        InstalledEnabledMods.Add(System.IO.Path.GetFileName(modPath), App.path + $@"\Mods\Custom uMods\{System.IO.Path.GetFileName(modPath)}");
                    }
                    else
                    {
                        EnabledMods.Add(System.IO.Path.GetFileName(modPath));
                        InstalledEnabledMods.Add(System.IO.Path.GetFileName(modPath), App.path + $@"\Mods\Custom uMods\{System.IO.Path.GetFileName(modPath)}");
                    }
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Removing mods
        private async void RemoveSelectedMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnabledModsList.SelectedItem != null)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete mod files?", "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        await RemoveMod(System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(InstalledEnabledMods[EnabledModsList.SelectedItem.ToString()])), InstalledEnabledMods[EnabledModsList.SelectedItem.ToString()]);
                    }
                    InstalledEnabledMods.Remove(EnabledModsList.SelectedItem.ToString());
                    EnabledMods.Remove(EnabledModsList.SelectedItem.ToString());
                }
                if (DisabledModsList.SelectedItem != null)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete mod files?", "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        await RemoveMod(System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(InstalledDisabledMods[DisabledModsList.SelectedItem.ToString()])), InstalledDisabledMods[DisabledModsList.SelectedItem.ToString()]);
                    }
                    InstalledDisabledMods.Remove(DisabledModsList.SelectedItem.ToString());
                    DisabledMods.Remove(DisabledModsList.SelectedItem.ToString());
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async Task RemoveMod(string folderName, string modPath)
        {
            try
            {
                switch (folderName)
                {
                    case "Overhaul":
                    case "PS3Buttons":
                        if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(modPath)))
                        {
                            System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(modPath), true);
                        }
                        break;
                    default:
                        if (System.IO.File.Exists(modPath))
                        {
                            System.IO.File.Delete(modPath);
                        }
                        break;
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async void MoveSelectedModUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnabledModsList.SelectedItem != null)
                {
                    int selectedIndex = EnabledModsList.SelectedIndex;
                    if (EnabledModsList.SelectedIndex > 0)
                    {
                        string selectedItem = EnabledMods[selectedIndex];
                        EnabledMods.RemoveAt(selectedIndex);
                        EnabledMods.Insert(selectedIndex - 1, selectedItem);
                        EnabledModsList.SelectedIndex = selectedIndex - 1;
                    }
                }
                if (DisabledModsList.SelectedItem != null)
                {
                    int selectedIndex = DisabledModsList.SelectedIndex;
                    if (DisabledModsList.SelectedIndex > 0)
                    {
                        string selectedItem = DisabledMods[selectedIndex];
                        DisabledMods.RemoveAt(selectedIndex);
                        DisabledMods.Insert(selectedIndex - 1, selectedItem);
                        DisabledModsList.SelectedIndex = selectedIndex - 1;
                    }
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Moves selected item down 1 spot
        private async void MoveSelectedModDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnabledModsList.SelectedItem != null)
                {
                    int selectedIndex = EnabledModsList.SelectedIndex;
                    if (EnabledModsList.SelectedIndex >= 0 && EnabledModsList.SelectedIndex < (EnabledMods.Count - 1))
                    {
                        string selectedItem = EnabledMods[selectedIndex];
                        EnabledMods.RemoveAt(selectedIndex);
                        EnabledMods.Insert(selectedIndex + 1, selectedItem.ToString());
                        EnabledModsList.SelectedIndex = selectedIndex + 1;
                    }
                }
                if (DisabledModsList.SelectedItem != null)
                {
                    int selectedIndex = DisabledModsList.SelectedIndex;
                    if (DisabledModsList.SelectedIndex >= 0 && DisabledModsList.SelectedIndex < (DisabledMods.Count - 1))
                    {
                        string selectedItem = DisabledMods[selectedIndex];
                        DisabledMods.RemoveAt(selectedIndex);
                        DisabledMods.Insert(selectedIndex + 1, selectedItem);
                        DisabledModsList.SelectedIndex = selectedIndex + 1;
                    }
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // This is needed to have only 1 mod selected
        private void EnabledModsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isSelectionEnabledModsChangingProgrammatically)
            {
                isSelectionDisabledModsChangingProgrammatically = true;
                DisabledModsList.SelectedIndex = -1;
                isSelectionDisabledModsChangingProgrammatically = false;
            }
        }

        private void DisabledModsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isSelectionDisabledModsChangingProgrammatically)
            {
                isSelectionEnabledModsChangingProgrammatically = true;
                EnabledModsList.SelectedIndex = -1;
                isSelectionEnabledModsChangingProgrammatically = false;
            }
        }

        // Moves selected mod into enabled mod list
        private async void EnableSelectedMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DisabledModsList.SelectedIndex > -1)
                {
                    EnabledMods.Add(DisabledModsList.SelectedItem.ToString());
                    InstalledEnabledMods.Add(DisabledModsList.SelectedItem.ToString(), InstalledDisabledMods[DisabledModsList.SelectedItem.ToString()]);
                    InstalledDisabledMods.Remove(DisabledModsList.SelectedItem.ToString());
                    DisabledMods.Remove(DisabledModsList.SelectedItem.ToString());
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Moves selected mod into disabled mod list
        private async void DisableSelectedMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnabledModsList.SelectedIndex > -1)
                {
                    DisabledMods.Add(EnabledModsList.SelectedItem.ToString());
                    InstalledDisabledMods.Add(EnabledModsList.SelectedItem.ToString(), InstalledEnabledMods[EnabledModsList.SelectedItem.ToString()]);
                    InstalledEnabledMods.Remove(EnabledModsList.SelectedItem.ToString());
                    EnabledMods.Remove(EnabledModsList.SelectedItem.ToString());
                }
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
