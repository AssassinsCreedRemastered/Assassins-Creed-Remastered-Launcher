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
using Microsoft.VisualBasic.Logging;
using System.Security.Principal;

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
            FillCheckBox();
        }

        // Global
        List<Resolution> compatibleResolutions = new List<Resolution>();
        private string path = "";

        // Functions
        private void FillCheckBox()
        {
            KeyboardLayoutSelector.Items.Add("KeyboardMouse2");
            KeyboardLayoutSelector.Items.Add("KeyboardMouse5");
            KeyboardLayoutSelector.Items.Add("Keyboard");
            KeyboardLayoutSelector.Items.Add("KeyboardAlt");
            GC.Collect();
        }

        // Get Path where AC installation is
        private async void GetDirectory()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Path.txt"))
                {
                    path = sr.ReadLine() + @"\";
                }
                GC.Collect();
                await Task.Delay(1);
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
                    string? line = sr.ReadLine();
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
                GC.Collect();
                await Task.Delay(1);
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
                await CheckReShade();
                if (App.uModStatus)
                {
                    uMod.IsChecked = true;
                }
                else
                {
                    uMod.IsChecked = false;
                }
                await CheckEaglePatch();
                GC.Collect();
                await Task.Delay(1);
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
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
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
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Check if EaglePatch is enabled
        private async Task CheckEaglePatch()
        {
            try
            {
                if (System.IO.File.Exists(App.path + @"\dinput8.dll"))
                {
                    EaglePatch.IsChecked = true;
                }
                await ReadEaglePatchConfig();
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Read EaglePatch Configuration File
        private async Task ReadEaglePatchConfig()
        {
            try
            {
                if (System.IO.File.Exists(App.path + @"\scripts\EaglePatchAC1.ini"))
                {
                    string[] EaglePatchConfig = File.ReadAllLines(App.path + @"\scripts\EaglePatchAC1.ini");
                    foreach (string line in EaglePatchConfig)
                    {
                        List<string> splitLine = new List<string>();
                        switch (line)
                        {
                            case string x when line.StartsWith("KeyboardLayout"):
                                splitLine = line.Split('=').ToList();
                                KeyboardLayoutSelector.SelectedIndex = int.Parse(splitLine[1]);
                                splitLine.Clear();
                                break;
                            case string x when line.StartsWith("PS3Controls"):
                                splitLine = line.Split('=').ToList();
                                if (int.Parse(splitLine[1]) == 1)
                                {
                                    PS3Controls.IsChecked = true;
                                }
                                splitLine.Clear();
                                break;
                            case string x when line.StartsWith("SkipIntroVideos"):
                                splitLine = line.Split('=').ToList();
                                if (int.Parse(splitLine[1]) == 1)
                                {
                                    SkipIntroVideos.IsChecked = true;
                                }
                                splitLine.Clear();
                                break;
                            default:
                                break;
                        }
                    }
                }
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                        string? line = sr.ReadLine();
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
                GC.Collect();
                await Task.Delay(1);
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
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        private async Task SaveuModStatus()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path + @"\uMod\Status.txt"))
                {
                    if (uMod.IsChecked == true)
                    {
                        App.uModStatus = true;
                        sw.Write("Enabled=1");
                    } else
                    {
                        App.uModStatus = false;
                        sw.Write("Enabled=0");
                    }
                }
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        private async Task SaveEaglePatchSettings()
        {
            try
            {
                string[] EaglePatchConfigFile = File.ReadAllLines(App.path + @"\scripts\EaglePatchAC1.ini");
                using (StreamWriter sw = new StreamWriter(App.path + @"\scripts\EaglePatchAC1.ini"))
                {
                    foreach (string line in EaglePatchConfigFile)
                    {
                        switch (line)
                        {
                            case string x when line.StartsWith("KeyboardLayout"):
                                sw.Write("KeyboardLayout=" + KeyboardLayoutSelector.SelectedIndex + "\r\n");
                                break;
                            case string x when line.StartsWith("PS3Controls"):
                                if (PS3Controls.IsChecked == true)
                                {
                                    sw.Write("PS3Controls=1\r\n");
                                }
                                else
                                {
                                    sw.Write("PS3Controls=0\r\n");
                                }
                                break;
                            case string x when line.StartsWith("SkipIntroVideos"):
                                if (SkipIntroVideos.IsChecked == true)
                                {
                                    sw.Write("SkipIntroVideos=1\r\n");
                                }
                                else
                                {
                                    sw.Write("SkipIntroVideos=0\r\n");
                                }
                                break;
                            default:
                                sw.Write(line + "\r\n");
                                break;
                        }
                    }
                }
                if (EaglePatch.IsChecked == true)
                {
                    if (System.IO.File.Exists(App.path + @"\dinput8.dll.disabled"))
                    {
                        System.IO.File.Move(App.path + @"\dinput8.dll.disabled", App.path + @"\dinput8.dll");
                    }
                }
                else
                {
                    if (System.IO.File.Exists(App.path + @"\dinput8.dll"))
                    {
                        System.IO.File.Move(App.path + @"\dinput8.dll", App.path + @"\dinput8.dll.disabled");
                    }
                }
                GC.Collect();
                await Task.Delay(1);
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
            GC.Collect();
        }

        // Disabling server calls
        private void HostsFix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    string hostsFilePath = System.IO.Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
                    using (StreamWriter sw = File.AppendText(hostsFilePath))
                    {
                        sw.WriteLine("203.132.26.155 127.0.0.1");
                    }
                    System.Windows.MessageBox.Show("Fix has been applied.");
                }
                else
                {
                    System.Windows.MessageBox.Show("Run the launcher as Administrator to use this.");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }

        // Save Settings
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SaveGameSettings();
                await SaveReShade();
                await SaveuModStatus();
                await SaveEaglePatchSettings();
                GC.Collect();
                await Task.Delay(1);
                System.Windows.MessageBox.Show("Changes are saved");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
