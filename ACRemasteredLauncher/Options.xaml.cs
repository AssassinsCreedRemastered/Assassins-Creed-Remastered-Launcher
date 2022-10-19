using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Xaml;
using Microsoft.Win32;
using System.Windows.Interop;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Markup.Localizer;
using System.Collections;

namespace ACRemasteredLauncher
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        List<Resolutions> SupportedResolutions = new List<Resolutions>();
        List<string> ReShade = new List<string>();
        Dictionary<string, List<string>> ReShadePresets = new Dictionary<string, List<string>>();
        Resolutions MonitorsSpecifications = new Resolutions();
        string InstallationFolder;
        string CurrentIntroQuality;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Options()
        {
            InitializeComponent();
            CheckGameConfig();
            LauncherConfig();
            FillComboBoxes();
            FindResolution();
            LoadSettings();
            GC.Collect();
        }

        //Load Settings
        private void CheckGameConfig()
        {
            Logger.Debug("Checking Game Config");
            if (!System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini"))
            {
                FindMonitor fm = new FindMonitor();
                fm.ShowDialog();
                Logger.Debug("Finding Monitor's Resolution and Refresh Rate");
                string MonitorSpecs = fm.MonitorsSpecs.Text;
                Logger.Debug(MonitorSpecs);
                fm.Close();
                string[] resolution = MonitorSpecs.Split(';');
                string[] widthHeight = resolution[0].Split('x');
                MonitorsSpecifications.Resolution = resolution[0];
                MonitorsSpecifications.RefreshRate.Add(int.Parse(resolution[1]));
                MonitorsSpecifications.Width = double.Parse(widthHeight[0]);
                MonitorsSpecifications.Height = double.Parse(widthHeight[1]);
                string defaultConfig = Properties.Resources.Assassin;
                List<string> defaultConfigAsList = defaultConfig.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini"))
                {
                    foreach (string line in defaultConfigAsList)
                    {
                        if (line.StartsWith("DisplayWidth"))
                        {
                            
                        }
                        switch (line)
                        {
                            default:
                                sw.Write(line + "\r\n");
                                break;
                            case string x when x.StartsWith("DisplayWidth"):
                                sw.Write("DisplayWidth=" + MonitorsSpecifications.Width + "\r\n");
                                break;
                            case string x when x.StartsWith("DisplayHeight"):
                                sw.Write("DisplayHeight=" + MonitorsSpecifications.Height + "\r\n");
                                break;
                            case string x when x.StartsWith("RefreshRate"):
                                sw.Write("RefreshRate=" + MonitorsSpecifications.RefreshRate[0] + "\r\n");
                                break;
                        }
                    }
                }
                Logger.Debug("Checking Game Config - DONE");
                GC.Collect();
            }
        }

        private void LauncherConfig()
        {
            Logger.Debug("Checking Launcher Configuration");
            string[] LauncherConfig = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Launcher.config");
            foreach (string config in LauncherConfig)
            {
                List<string> split = new List<string>();
                switch (config)
                {
                    default:
                        break;
                    case string InstallationPath when InstallationPath.StartsWith("InstallationDirectory="):
                        split = config.Split('=').ToList();
                        InstallationFolder = split[1];
                        split.Clear();
                        break;
                }
            }
            Logger.Debug("Checking Launcher Config - DONE");
            GC.Collect();
        }
        private void FillComboBoxes()
        {
            FindInstalledReShadePresets();
            GraphicsModSelection.Items.Add("Original");
            GraphicsModSelection.Items.Add("CryNation");
            GraphicsModSelection.Items.Add("Overhaul");
            IntroQualitySelection.Items.Add("Original");
            IntroQualitySelection.Items.Add("4k with Color");
            IntroQualitySelection.Items.Add("4k Colorless");
        }

        private void FindInstalledReShadePresets()
        {
            Logger.Debug("Finding every installed ReShade Preset");
            DirectoryInfo presetsDirectory = new DirectoryInfo(InstallationFolder + @"\reshade-presets\");
            DirectoryInfo[] directories = presetsDirectory.GetDirectories();
            foreach (DirectoryInfo item in directories)
            {
                List<string> temp = new List<string>();
                FileInfo[] fileInfo = item.GetFiles();
                string save = "";
                switch (item.Name)
                {
                    default:
                        save = "Custom";
                        foreach (FileInfo fileitem in fileInfo)
                        {
                            temp.Add(fileitem.Name.Replace(".ini",""));
                        }
                        break;
                    case "CryNation":
                        save = "CryNation";
                        foreach (FileInfo fileitem in fileInfo)
                        {
                            temp.Add(fileitem.Name.Replace(".ini", ""));
                        }
                        break;
                    case "Overhaul":
                        save = "Overhaul";
                        foreach (FileInfo fileitem in fileInfo)
                        {
                            temp.Add(fileitem.Name.Replace(".ini", ""));
                        }
                        break;
                }
                Logger.Debug("Finding every installed ReShade Preset - DONE");
                ReShadePresets.Add(save, temp);
                GC.Collect();
            }
        }
        private void FindResolution()
        {
            Logger.Debug("Finding every resolution supported by the monitor");
            FindMonitor fm = new FindMonitor();
            fm.ShowDialog();
            string MonitorSpecs = fm.MonitorsSpecs.Text;
            fm.Close();
            string[] resolution = MonitorSpecs.Split(';');
            string[] widthHeight = resolution[0].Split('x');
            MonitorsSpecifications.Resolution = resolution[0];
            MonitorsSpecifications.RefreshRate.Add(int.Parse(resolution[1]));
            MonitorsSpecifications.Width = double.Parse(widthHeight[0]);
            MonitorsSpecifications.Height = double.Parse(widthHeight[1]);
            using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ACRemasteredLauncher.ListofSupportedResolutions.txt")))
            {
                //640x480;59
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] split = line.Split(';');
                    string[] splitWidthHeight = split[0].Split('x');
                    double tempWidth = double.Parse(splitWidthHeight[0]);
                    double tempHeight = double.Parse(splitWidthHeight[1]);
                    if (tempWidth <= MonitorsSpecifications.Width && tempHeight <= MonitorsSpecifications.Height)
                    {
                        if (SupportedResolutions.Count < 1)
                        {
                            Resolutions newRes = new Resolutions();
                            newRes.Resolution = split[0];
                            newRes.RefreshRate.Add(int.Parse(split[1]));
                            newRes.Width = tempWidth;
                            newRes.Height = tempHeight;
                            SupportedResolutions.Add(newRes);
                        } else
                        {
                            bool newResolution = true;
                            foreach (Resolutions item in SupportedResolutions)
                            {
                                if (item.Resolution == split[0])
                                {
                                    newResolution = false;
                                    if (!item.RefreshRate.Contains(int.Parse(split[1])))
                                    {
                                        item.RefreshRate.Add(int.Parse(split[1]));
                                    }
                                    break;
                                }
                            }
                            if (newResolution)
                            {
                                Resolutions newRes = new Resolutions();
                                newRes.Resolution = split[0];
                                newRes.RefreshRate.Add(int.Parse(split[1]));
                                newRes.Width = tempWidth;
                                newRes.Height = tempHeight;
                                SupportedResolutions.Add(newRes);
                            }
                        }
                    }
                    line = sr.ReadLine();
                }
                GC.Collect();
            }
            Logger.Debug("Finding every resolution supported by the monitor - DONE");
            Logger.Debug("Addding every resolution supported by the monitor to a ComboBox");
            foreach (Resolutions item in SupportedResolutions)
            {
                if (item.RefreshRate[0] <= MonitorsSpecifications.RefreshRate[0])
                {
                    ResolutionsList.Items.Add(item.Resolution);
                }
            }
            ResolutionsList.SelectedIndex = ResolutionsList.Items.Count - 1;
            Logger.Debug("Addding every resolution supported by the monitor to a ComboBox - DONE");
            GC.Collect();
        }
        private void LoadSettings()
        {
            Logger.Debug("Loading all settings");
            string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini");
            string currentResolution = "";
            foreach (string line in lines)
            {
                List<string> split = new List<string>();
                switch (line)
                {
                    default:
                        break;
                    case string displayWidth when displayWidth.StartsWith("DisplayWidth"):
                        split = line.Split('=').ToList();
                        currentResolution = split[1];
                        split.Clear();
                        break;
                    case string displayHeight when displayHeight.StartsWith("DisplayHeight"):
                        split = line.Split('=').ToList();
                        currentResolution = currentResolution + "x" + split[1];
                        split.Clear();
                        break;
                    case string aa when aa.StartsWith("Multisampling"):
                        split = line.Split('=').ToList();
                        AntialiasingList.SelectedIndex = int.Parse(split[1]);
                        split.Clear();
                        break;
                    case string vSync when vSync.StartsWith("VSynch"):
                        split = line.Split('=').ToList();
                        VSync.SelectedIndex = int.Parse(split[1]);
                        split.Clear();
                        break;
                    case string fullScreen when fullScreen.StartsWith("Fullscreen"):
                        split = line.Split('=').ToList();
                        if (int.Parse(split[1]) == 1)
                        {
                            FullScreen.IsChecked = true;
                        } else
                        {
                            FullScreen.IsChecked = false;
                        }
                        split.Clear();
                        break;
                }
            }
            if (ResolutionsList.Items.Contains(currentResolution))
            {
                ResolutionsList.SelectedIndex = ResolutionsList.Items.IndexOf(currentResolution);
            } else
            {
                ResolutionsList.SelectedIndex = ResolutionsList.Items.Count - 1;
            }
            GC.Collect();
            ReadConfigFiles();
            CurrentDemoQuality();
            Logger.Debug("Loading all settings - DONE");
        }
        private void ReadConfigFiles()
        {
            string[] uModConfig = File.ReadAllLines(InstallationFolder + @"\Mods\uMod\templates\ac1.txt");
            string[] ReShadeConfig = File.ReadAllLines(InstallationFolder + @"\ReShade.ini");
            string[] EaglePatchConfig = File.ReadAllLines(InstallationFolder + @"\scripts\EaglePatchAC1.ini");
            bool GraphicsMod = false;
            Logger.Debug("Reading uMod Config");
            foreach (string line in uModConfig)
            {
                if (line.StartsWith("Add_true:"))
                {
                    if (line.EndsWith("Assassin's Creed Overhaul 2016 Full Version.tpf"))
                    {
                        GraphicsMod = true;
                        GraphicsModSelection.SelectedIndex = 2;
                    }
                    else if (line.EndsWith("AC1 CryNation.tpf"))
                    {
                        GraphicsMod = true;
                        GraphicsModSelection.SelectedIndex = 1;
                    }
                    else if (line.EndsWith("AC1 PS Buttons.tpf"))
                    {
                        PS3Icons.IsChecked = true;
                    }
                }
            }
            Logger.Debug("Reading uMod Config - DONE");
            GC.Collect();
            if (!GraphicsMod)
            {
                GraphicsModSelection.SelectedIndex = 0;
            }
            Logger.Debug("Reading ReShade Config");
            foreach (string line in ReShadeConfig)
            {
                if (line.StartsWith("PresetPath"))
                {
                    string[] tempSplit = line.Split('=');
                    string test;
                    if (GraphicsModSelection.SelectedItem.ToString() == "Original")
                    {
                        test = tempSplit[1].Replace(".\\reshade-presets\\Custom\\", "");
                        test = test.Replace(".ini", "");
                    } else
                    {
                        test = tempSplit[1].Replace(".\\reshade-presets\\" + GraphicsModSelection.SelectedItem.ToString() + @"\", "");
                        test = test.Replace(".ini", "");

                    }
                    if (test.Length < 1)
                    {
                        ReShadePreset.SelectedIndex = -1;
                    } else
                    {
                        ReShadePreset.SelectedItem = test;
                    }
                }
                if (line.StartsWith("ShowFPS"))
                {
                    string[] tempSplit = line.Split('=');
                    if (int.Parse(tempSplit[1]) == 1)
                    {
                        ShowFPS.IsChecked = true;
                    }
                    else
                    {
                        ShowFPS.IsChecked = false;
                    }
                }
                if (line.StartsWith("ShowFrameTime"))
                {
                    string[] tempSplit = line.Split('=');
                    if (int.Parse(tempSplit[1]) == 1)
                    {
                        ShowFrameTime.IsChecked = true;
                    }
                    else
                    {
                        ShowFrameTime.IsChecked = false;
                    }
                    break;
                }
            }
            Logger.Debug("Reading ReShade Config - DONE");
            GC.Collect();
            Logger.Debug("Reading EaglePatch Config");
            foreach (string line in EaglePatchConfig)
            {
                if (line.StartsWith("PS3Controls"))
                {
                    string[] tempSplit = line.Split('=');
                    if (int.Parse(tempSplit[1]) == 1)
                    {
                        SwapTriggersAndBumpers.IsChecked = true;
                    }
                    else
                    {
                        SwapTriggersAndBumpers.IsChecked = false;
                    }
                }
                else if (line.StartsWith("SkipIntroVideos"))
                {
                    string[] tempSplit = line.Split('=');
                    if (int.Parse(tempSplit[1]) == 1)
                    {
                        SkipIntro.IsChecked = true;
                    }
                    else
                    {
                        SkipIntro.IsChecked = false;
                    }
                }
            }
            Logger.Debug("Reading EaglePatch Config - DONE");
        }
        private void CurrentDemoQuality()
        {
            Logger.Debug("Finding current Intro Demo Quality");
            List<string> tempList = new List<string>();
            DirectoryInfo d = new DirectoryInfo(InstallationFolder + @"\Videos");
            FileInfo[] files = d.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.StartsWith("DemoIntro"))
                {
                    tempList.Add(file.Name);
                }
            }
            GC.Collect();
            if (!tempList.Contains("DemoIntro1080p.bik"))
            {
                IntroQualitySelection.SelectedIndex = 0;
            }
            else if (!tempList.Contains("DemoIntro4kColor.bik"))
            {
                IntroQualitySelection.SelectedIndex = 1;
            } else
            {
                IntroQualitySelection.SelectedIndex = 2;
            }
            CurrentIntroQuality = IntroQualitySelection.SelectedItem.ToString();
            Logger.Debug("Finding current Intro Demo Quality - DONE");
        }

        //Save Settings Functions

        private void SaveSettings()
        {
            SaveDisplay();
            GC.Collect();
            SaveuModTextTemplate();
            GC.Collect();
            SaveReShade();
            GC.Collect();
            SaveEaglePatch();
            GC.Collect();
            SaveQualityIntro();
            SaveLauncherConfig();
        }

        private void SaveDisplay()
        {
            Logger.Debug("Saving Display Settings");
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
                                foreach (Resolutions resolution in SupportedResolutions)
                                {
                                    if (resolution.Resolution == ResolutionsList.SelectedItem.ToString())
                                    {
                                        sw.Write("DisplayWidth=" + resolution.Width + "\r\n");
                                        break;
                                    }
                                }
                                break;
                            case string x when line.StartsWith("DisplayHeight"):
                                foreach (Resolutions resolution in SupportedResolutions)
                                {
                                    if (resolution.Resolution == ResolutionsList.SelectedItem.ToString())
                                    {
                                        sw.Write("DisplayHeight=" + resolution.Height + "\r\n");
                                        break;
                                    }
                                }
                                break;
                            case string x when line.StartsWith("RefreshRate"):
                                sw.Write("RefreshRate=" + RefreshRateList.SelectedItem.ToString() +"\r\n");
                                break;
                            case string x when line.StartsWith("Multisampling"):
                                sw.Write("Multisampling=" + AntialiasingList.SelectedIndex + "\r\n");
                                break;
                            case string x when line.StartsWith("VSynch"):
                                sw.Write("VSynch=" + VSync.SelectedIndex + "\r\n");
                                break;
                            case string x when line.StartsWith("Fullscreen"):
                                if (FullScreen.IsChecked == true)
                                {
                                    sw.Write("Fullscreen=1" + "\r\n");
                                } else
                                {
                                    sw.Write("Fullscreen=0" + "\r\n");
                                }
                                break;
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini");
            File.Move(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\AssassinTemp.ini", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Assassin.ini");
            Logger.Debug("Saving Display Settings - DONE");
        }

        private void SaveuModTextTemplate()
        {
            Logger.Debug("Saving uMod Settings");
            //"Add_true:" + InstallationDirectory + @"\Mods\AC1 Overhaul\Assassin's Creed Overhaul 2016 Full Version.tpf" + "\n"
            using (StreamReader sr = new StreamReader(InstallationFolder + @"\Mods\uMod\templates\ac1.txt"))
            {
                using (StreamWriter sw = new StreamWriter(InstallationFolder + @"\Mods\uMod\templates\ac1temp.txt"))
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
                            sw.Write(line + "\n");
                        }
                        line = sr.ReadLine();
                    }
                    switch (GraphicsModSelection.SelectedItem.ToString())
                    {
                        default:
                            break;
                        case "CryNation":
                            sw.Write("Add_true:" + InstallationFolder + @"\Mods\CryNation\AC1 CryNation.tpf" + "\n");
                            break;
                        case "Overhaul":
                            sw.Write("Add_true:" + InstallationFolder + @"\Mods\AC1 Overhaul\Assassin's Creed Overhaul 2016 Full Version.tpf" + "\n");
                            break;
                    }
                    if (PS3Icons.Visibility == Visibility.Visible && PS3Icons.IsChecked == true)
                    {
                        sw.Write("Add_true:" + InstallationFolder + @"\Mods\PS3 Buttons\AC1 PS Buttons.tpf" + "\n");
                    }
                }
            }
            File.Delete(InstallationFolder + @"\Mods\uMod\templates\ac1.txt");
            File.Move(InstallationFolder + @"\Mods\uMod\templates\ac1temp.txt", InstallationFolder + @"\Mods\uMod\templates\ac1.txt");
            Logger.Debug("Saving uMod Settings - DONE");
            GC.Collect();
        }

        private void SaveReShade()
        {
            Logger.Debug("Saving ReShade Settings");
            using (StreamReader sr = new StreamReader(InstallationFolder + @"\ReShade.ini"))
            {
                using (StreamWriter sw = new StreamWriter(InstallationFolder + @"\ReShadeTemp.ini"))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        if (line.StartsWith("PresetPath"))
                        {
                            if (ReShadePreset.Visibility == Visibility.Visible && ReShadePreset.SelectedIndex > -1)
                            {
                                if (GraphicsModSelection.SelectedItem.ToString() == "Original")
                                {
                                    sw.Write("PresetPath=.\\reshade-presets\\Custom\\" + ReShadePreset.SelectedItem.ToString() + ".ini" + "\r\n");
                                } else
                                {
                                    sw.Write("PresetPath=.\\reshade-presets\\" + GraphicsModSelection.SelectedItem.ToString() + @"\" + ReShadePreset.SelectedItem.ToString() + ".ini" + "\r\n");
                                }
                            } 
                            else
                            {
                                sw.Write("PresetPath=.\\reshade-presets\\" + "\r\n");
                            }
                        }
                        else if (line.StartsWith("ShowFPS"))
                        {
                            if (ShowFPS.Visibility == Visibility.Visible && ShowFPS.IsChecked == true)
                            {
                                sw.Write("ShowFPS=1" + "\r\n");
                            } 
                            else
                            {
                                sw.Write("ShowFPS=0" + "\r\n");
                            }
                        }
                        else if (line.StartsWith("ShowFrameTime"))
                        {
                            if (ShowFrameTime.Visibility == Visibility.Visible && ShowFrameTime.IsChecked == true)
                            {
                                sw.Write("ShowFrameTime=1" + "\r\n");
                            } 
                            else
                            {
                                sw.Write("ShowFrameTime=0" + "\r\n");
                            }
                        } 
                        else
                        {
                            sw.Write(line + "\r\n");
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            File.Delete(InstallationFolder + @"\ReShade.ini");
            File.Move(InstallationFolder + @"\ReShadeTemp.ini", InstallationFolder + @"\ReShade.ini");
            Logger.Debug("Saving ReShade Settings - DONE");
            GC.Collect();
        }

        private void SaveEaglePatch()
        {
            Logger.Debug("Saving EaglePatch Settings");
            using (StreamReader sr = new StreamReader(InstallationFolder + @"\scripts\EaglePatchAC1.ini"))
            {
                using (StreamWriter sw = new StreamWriter(InstallationFolder + @"\scripts\EaglePatchAC1temp.ini"))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        if (line.StartsWith("PS3Controls"))
                        {
                            if (SwapTriggersAndBumpers.IsChecked == true)
                            {
                                sw.Write("PS3Controls=1" + "\r\n");
                            } 
                            else
                            {
                                sw.Write("PS3Controls=0" + "\r\n");
                            }
                        }
                        else if (line.StartsWith("SkipIntroVideos"))
                        {
                            if (SkipIntro.IsChecked == true)
                            {
                                sw.Write("SkipIntroVideos=1" + "\r\n");
                            } 
                            else
                            {
                                sw.Write("SkipIntroVideos=0" + "\r\n");
                            }
                        } 
                        else
                        {
                            sw.Write(line + "\r\n");
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            File.Delete(InstallationFolder + @"\scripts\EaglePatchAC1.ini");
            File.Move(InstallationFolder + @"\scripts\EaglePatchAC1temp.ini", InstallationFolder + @"\scripts\EaglePatchAC1.ini");
            Logger.Debug("Saving EaglePatch Settings - DONE");
            GC.Collect();
        }

        private void SaveQualityIntro()
        {
            Logger.Debug("Saving Intro Quality");
            if (IntroQualitySelection.SelectedItem.ToString() != CurrentIntroQuality)
            {
                switch (CurrentIntroQuality)
                {
                    default:
                        break;
                    case "Original":
                        File.Move(InstallationFolder + @"\Videos\DemoIntro.bik", InstallationFolder + @"\Videos\DemoIntro1080p.bik");
                        switch (IntroQualitySelection.SelectedItem.ToString())
                        {
                            default:
                                File.Move(InstallationFolder + @"\Videos\DemoIntro1080p.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                            case "4k with Color":
                                File.Move(InstallationFolder + @"\Videos\DemoIntro4kColor.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                            case "4k Colorless":
                                File.Move(InstallationFolder + @"\Videos\DemoIntro4kColorless.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                        }
                        break;
                    case "4k with Color":
                        File.Move(InstallationFolder + @"\Videos\DemoIntro.bik", InstallationFolder + @"\Videos\DemoIntro4kColor.bik");
                        switch (IntroQualitySelection.SelectedItem.ToString())
                        {
                            default:
                                File.Move(InstallationFolder + @"\Videos\DemoIntro1080p.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                            case "4k with Color":
                                File.Move(InstallationFolder + @"\Videos\DemoIntro4kColor.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                            case "4k Colorless":
                                File.Move(InstallationFolder + @"\Videos\DemoIntro4kColorless.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                        }
                        break;
                    case "4k Colorless":
                        File.Move(InstallationFolder + @"\Videos\DemoIntro.bik", InstallationFolder + @"\Videos\DemoIntro4kColorless.bik");
                        switch (IntroQualitySelection.SelectedItem.ToString())
                        {
                            default:
                                File.Move(InstallationFolder + @"\Videos\DemoIntro1080p.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                            case "4k with Color":
                                File.Move(InstallationFolder + @"\Videos\DemoIntro4kColor.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                            case "4k Colorless":
                                File.Move(InstallationFolder + @"\Videos\DemoIntro4kColorless.bik", InstallationFolder + @"\Videos\DemoIntro.bik");
                                break;
                        }
                        break;
                }
            }
            Logger.Debug("Saving Intro Quality - DONE");
            GC.Collect();
        }

        private void SaveLauncherConfig()
        {
            Logger.Debug("Saving Launcher Config");
            using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Launcher.config"))
            {
                using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\LauncherTemp.config"))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        switch (line)
                        {
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Launcher.config");
            File.Move(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\LauncherTemp.config", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Launcher.config");
            Logger.Debug("Saving Launcher Config - DONE");
        }

        //Events
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            GC.Collect();
            this.Visibility = Visibility.Hidden;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            this.Visibility = Visibility.Hidden;
        }

        private void ResolutionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResolutionsList.SelectedIndex < 0)
            {
                return;
            }
            string selectedResolution;
            try
            {
                selectedResolution = ResolutionsList.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            RefreshRateList.Items.Clear();
            foreach (Resolutions item in SupportedResolutions)
            {
                if (item.Resolution == selectedResolution)
                {
                    foreach (int RefreshRate in item.RefreshRate)
                    {
                        if (RefreshRate <= MonitorsSpecifications.RefreshRate[0])
                        {
                            RefreshRateList.Items.Add(RefreshRate);
                        }
                    }
                    break;
                }
            }
            int itemsCount = RefreshRateList.Items.Count;
            RefreshRateList.SelectedIndex = itemsCount - 1;
        }

        private void GraphicsModSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GraphicsModSelection.SelectedIndex < 0)
            {
                return;
            }
            switch (GraphicsModSelection.SelectedItem.ToString())
            {
                default:
                    break;
                case "CryNation":
                    ReShadeLabel.Visibility = Visibility.Visible;
                    ReShadePreset.Visibility = Visibility.Visible;
                    ShowFPS.Visibility = Visibility.Hidden;
                    ShowFrameTime.Visibility = Visibility.Hidden;
                    ReShadePreset.Items.Clear();
                    foreach (string item in ReShadePresets["CryNation"])
                    {
                        ReShadePreset.Items.Add(item);
                    }
                    break;
                case "Overhaul":
                    ReShadeLabel.Visibility = Visibility.Visible;
                    ReShadePreset.Visibility = Visibility.Visible;
                    ShowFPS.Visibility = Visibility.Hidden;
                    ShowFrameTime.Visibility = Visibility.Hidden;
                    ReShadePreset.Items.Clear();
                    foreach (string item in ReShadePresets["Overhaul"])
                    {
                        ReShadePreset.Items.Add(item);
                    }
                    ReShadePreset.SelectedIndex = 0;
                    break;
                case "Original":
                    ReShadePreset.Items.Clear();
                    if (ReShadePresets["Custom"].Count > 0)
                    {
                        ReShadeLabel.Visibility = Visibility.Visible;
                        ReShadePreset.Visibility = Visibility.Visible;
                        if (PS3Icons.IsChecked == true)
                        {
                            ShowFPS.Visibility = Visibility.Hidden;
                            ShowFrameTime.Visibility = Visibility.Hidden;
                        } else
                        {
                            ShowFPS.Visibility = Visibility.Visible;
                            ShowFrameTime.Visibility = Visibility.Visible;
                        }
                        foreach (string item in ReShadePresets["Custom"])
                        {
                            ReShadePreset.Items.Add(item);
                        }
                    } else
                    {
                        ReShadePreset.Visibility = Visibility.Hidden;
                        ReShadeLabel.Visibility = Visibility.Hidden;
                        if (PS3Icons.IsChecked == true)
                        {
                            ShowFPS.Visibility = Visibility.Hidden;
                            ShowFrameTime.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            ShowFPS.Visibility = Visibility.Visible;
                            ShowFrameTime.Visibility = Visibility.Visible;
                        }
                    }
                    break;
            }
        }

        private void ReShadePreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReShadePreset.SelectedIndex < 0)
            {
                ReShadePreset.SelectedIndex = 0;
                return;
            }
        }

        private void PS3Icons_Click(object sender, RoutedEventArgs e)
        {
            if (PS3Icons.IsChecked == true)
            {
                ShowFPS.Visibility = Visibility.Hidden;
                ShowFrameTime.Visibility = Visibility.Hidden;
            } else
            {
                ShowFPS.Visibility = Visibility.Visible;
                ShowFrameTime.Visibility = Visibility.Visible;
            }
        }
    }
}
