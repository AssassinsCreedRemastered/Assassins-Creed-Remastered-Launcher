using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Forms;
using System.Diagnostics;

namespace Assassins_Creed_Remastered_Launcher.Pages
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        public Options()
        {
            InitializeComponent();
        }

        // Global
        List<string> CustomuMods = new List<string>();
        List<Resolution> compatibleResolutions = new List<Resolution>();
        private string path = "";

        // Functions
        // Get Path where AC installation is
        private async void GetDirectory()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Path.txt"))
                {
                    path = sr.ReadLine() + @"\";
                }
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }


        // Used to find all of the supported resolutions
        private async Task FindSupportedResolutions()
        {
            try
            {
                Screen[] allScreens = Screen.AllScreens;
                int resolutionWidth = 0;
                int resolutionHeight = 0;

                foreach (Screen screen in allScreens)
                {
                    if (resolutionWidth < screen.Bounds.Width)
                    {
                        resolutionWidth = screen.Bounds.Width;
                        resolutionHeight = screen.Bounds.Height;
                    };
                }
                using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Assassins_Creed_Remastered_Launcher.ListofSupportedResolutions.txt")))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] splitLine = line.Split('x');
                        if (double.Parse(splitLine[0]) < resolutionWidth && double.Parse(splitLine[1]) < resolutionHeight)
                        {
                            Resolution newRes = new Resolution();
                            newRes.Res = line;
                            newRes.Width = double.Parse(splitLine[0]);
                            newRes.Height = double.Parse(splitLine[1]);
                            compatibleResolutions.Add(newRes);
                            ResolutionsList.Items.Add(newRes.Res);
                        }
                        else if (double.Parse(splitLine[0]) == resolutionWidth && double.Parse(splitLine[1]) == resolutionHeight)
                        {
                            Resolution newRes = new Resolution();
                            newRes.Res = line;
                            newRes.Width = double.Parse(splitLine[0]);
                            newRes.Height = double.Parse(splitLine[1]);
                            compatibleResolutions.Add(newRes);
                            ResolutionsList.Items.Add(newRes.Res);
                            ResolutionsList.SelectedIndex = ResolutionsList.Items.IndexOf(newRes.Res);
                            break;
                        };
                        line = sr.ReadLine();
                    };
                };
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Used to read all of the configuration files and setup the elements in the Options Page
        private async Task ReadConfigFiles()
        {
            try
            {
                await ReadGameConfig();
                await ReaduModConfig();
                await CheckReShade();
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Reads Assassin.ini for Game Settings
        private async Task ReadGameConfig()
        {
            try
            {
                string GameConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini";
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini"))
                {
                    string currentResolution = "";
                    string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini");
                    foreach (string line in lines)
                    {
                        List<string> split = new List<string>();
                        switch (line)
                        {
                            default:
                                break;
                            case string x when line.StartsWith("DisplayWidth"):
                                split = line.Split('=').ToList();
                                currentResolution = split[1];
                                split.Clear();
                                break;
                            case string x when line.StartsWith("DisplayHeight"):
                                split = line.Split('=').ToList();
                                currentResolution = currentResolution + "x" + split[1];
                                split.Clear();
                                break;
                            case string x when line.StartsWith("VSynch"):
                                split = line.Split('=').ToList();
                                if (int.Parse(split[1]) == 1)
                                {
                                    VSync.IsChecked = true;
                                }
                                else
                                {
                                    VSync.IsChecked = false;
                                }
                                split.Clear();
                                break;
                            case string x when line.StartsWith("Multisampling"):
                                split = line.Split('=').ToList();
                                if (int.Parse(split[1]) >= 0 && int.Parse(split[1]) <= 2)
                                {
                                    AntiAliasing.SelectedIndex = int.Parse(split[1]);
                                }
                                split.Clear();
                                break;
                        }
                    }
                    if (ResolutionsList.Items.Contains(currentResolution))
                    {
                        ResolutionsList.SelectedIndex = ResolutionsList.Items.IndexOf(currentResolution);
                    }
                    else
                    {
                        ResolutionsList.SelectedIndex = ResolutionsList.Items.Count - 1;
                    }
                }
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Reads uMod configuration
        private async Task ReaduModConfig()
        {
            try
            {
                if (File.Exists(path + @"uMod\templates\ac1.txt"))
                {
                    string[] uModConfig = File.ReadAllLines(path + @"uMod\templates\ac1.txt");
                    foreach (string line in uModConfig)
                    {
                        if (line.StartsWith("Add_true:"))
                        {
                            if (line.EndsWith("Overhaul Fixed For ReShade.tpf"))
                            {
                                OverhaulMod.IsChecked = true;
                            }
                            else if (line.EndsWith("AC1 PS Buttons.tpf"))
                            {
                                PS3Buttons.IsChecked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            await Task.Delay(10);
        }

        // Check if ReShade is enabled
        private async Task CheckReShade()
        {
            try
            {
                if (File.Exists(path + @"d3d9.dll") && File.Exists(path + @"dxgi.dll"))
                {
                    ReShade.IsChecked = true;
                }
                else
                {
                    ReShade.IsChecked = false;
                }
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Saving Game settings (Resolution, VSync, AntiAliasing)
        private async Task SaveGameSettings()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini"))
                {
                    using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\AssassinTemp.ini"))
                    {
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            switch (line)
                            {
                                default:
                                    sw.Write(line + "\r\n");
                                    break;
                                case string x when line.StartsWith("DisplayWidth"):
                                    foreach (Resolution resolution in compatibleResolutions)
                                    {
                                        if (resolution.Res == ResolutionsList.SelectedItem.ToString())
                                        {
                                            sw.Write("DisplayWidth=" + resolution.Width + "\r\n");
                                            break;
                                        }
                                    }
                                    break;
                                case string x when line.StartsWith("DisplayHeight"):
                                    foreach (Resolution resolution in compatibleResolutions)
                                    {
                                        if (resolution.Res == ResolutionsList.SelectedItem.ToString())
                                        {
                                            sw.Write("DisplayHeight=" + resolution.Height + "\r\n");
                                            break;
                                        }
                                    }
                                    break;
                                case string x when line.StartsWith("Multisampling"):
                                    sw.Write("Multisampling=" + AntiAliasing.SelectedIndex + "\r\n");
                                    break;
                                case string x when line.StartsWith("VSynch"):
                                    if (VSync.IsChecked == true)
                                    {
                                        sw.Write("VSynch=1" + "\r\n");
                                    }
                                    else
                                    {
                                        sw.Write("VSynch=0" + "\r\n");
                                    }
                                    break;
                            }
                            line = sr.ReadLine();
                        }
                    }
                }
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini");
                File.Move(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\AssassinTemp.ini", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini");
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        private async Task SaveUserAddeduMods()
        {
            try
            {
                CustomuMods.Clear();
                string[] customMods = Directory.GetFiles(path + @"\Mods\Custom uMods\");
                foreach (string customMod in customMods)
                {
                    if (customMod.EndsWith(".tpf"))
                    {
                        CustomuMods.Add(System.IO.Path.GetFileName(customMod));
                    }
                };
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        // Saving uMod settings (Overhaul mod, PSButtons etc..)
        private async Task SaveuModSettings()
        {
            try
            {
                using (StreamReader sr = new StreamReader(path + @"uMod\templates\ac1.txt"))
                {
                    using (StreamWriter sw = new StreamWriter(path + @"uMod\templates\ac1temp.txt"))
                    {
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            if (line.StartsWith("Add_true:"))
                            {
                                break;
                            }
                            else
                            {
                                sw.WriteLine(line);
                            }
                            line = sr.ReadLine();
                        }
                        if (OverhaulMod.IsChecked == true)
                        {
                            sw.Write("Add_true:" + path + @"Mods\Overhaul\Overhaul Fixed For ReShade.tpf" + "\n");
                        }
                        if (PS3Buttons.IsChecked == true)
                        {
                            sw.Write("Add_true:" + path + @"Mods\PS3Buttons\AC1 PS Buttons.tpf" + "\n");
                        }
                        if (CustomuMods.Count > 0)
                        {
                            foreach (string mod in CustomuMods)
                            {
                                sw.Write("Add_true:" + path + @"Mods\Custom uMods\" + mod + "\n");
                            }
                        }
                    }
                }
                File.Delete(path + @"uMod\templates\ac1.txt");
                File.Move(path + @"uMod\templates\ac1temp.txt", path + @"uMod\templates\ac1.txt");
                GC.Collect();
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Saving ReShade choice
        private async Task SaveReShade()
        {
            try
            {
                if (ReShade.IsChecked == true)
                {
                    if (File.Exists(path + @"d3d9.dll.disabled") && File.Exists(path + @"dxgi.dll.disabled"))
                    {
                        File.Move(path + @"d3d9.dll.disabled", path + @"d3d9.dll");
                        File.Move(path + @"dxgi.dll.disabled", path + @"dxgi.dll");
                    }
                }
                else
                {
                    if (File.Exists(path + @"d3d9.dll") && File.Exists(path + @"dxgi.dll"))
                    {
                        File.Move(path + @"d3d9.dll", path + @"d3d9.dll.disabled");
                        File.Move(path + @"dxgi.dll", path + @"dxgi.dll.disabled");
                    }
                }
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Events
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetDirectory();
            await FindSupportedResolutions();
            await ReadConfigFiles();
        }

        // Save Settings
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SaveUserAddeduMods();
                await SaveGameSettings();
                await SaveuModSettings();
                await SaveReShade();
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Opens Custom uMods folder
        private async void AddCustomuMods_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = path + @"Mods\Custom uMods\",
                    UseShellExecute = false
                };
                Process.Start(processInfo);
                System.Windows.MessageBox.Show("Drag all of the .tpf files in the open directory and then press OK.");
                await Task.Delay(10);
                await SaveUserAddeduMods();
                await SaveuModSettings();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
