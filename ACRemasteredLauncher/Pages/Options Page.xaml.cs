using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

namespace ACRemasteredLauncher.Pages
{
    /// <summary>
    /// Interaction logic for Options_Page.xaml
    /// </summary>
    public partial class Options_Page : Page
    {
        private string path = "";
        public Options_Page()
        {
            InitializeComponent();
        }

        // Global
        List<Resolutions> compatibleResolutions = new List<Resolutions>();

        // Functions
        // Used to find all of the supported resolutions
        private async Task FindSupportedResolutions()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ACRemasteredLauncher.ListofSupportedResolutions.txt")))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] splitLine = line.Split('x');
                        if (double.Parse(splitLine[0]) < System.Windows.SystemParameters.PrimaryScreenWidth && double.Parse(splitLine[1]) < System.Windows.SystemParameters.PrimaryScreenHeight)
                        {
                            Resolutions newRes = new Resolutions();
                            newRes.Resolution = line;
                            newRes.Width = double.Parse(splitLine[0]);
                            newRes.Height = double.Parse(splitLine[1]);
                            compatibleResolutions.Add(newRes);
                            ResolutionsList.Items.Add(newRes.Resolution);
                        }
                        else if (double.Parse(splitLine[0]) == System.Windows.SystemParameters.PrimaryScreenWidth && double.Parse(splitLine[1]) == System.Windows.SystemParameters.PrimaryScreenHeight)
                        {
                            Resolutions newRes = new Resolutions();
                            newRes.Resolution = line;
                            newRes.Width = double.Parse(splitLine[0]);
                            newRes.Height = double.Parse(splitLine[1]);
                            compatibleResolutions.Add(newRes);
                            ResolutionsList.Items.Add(newRes.Resolution);
                            ResolutionsList.SelectedIndex = ResolutionsList.Items.IndexOf(newRes.Resolution);
                            break;
                        };
                        line = sr.ReadLine();
                    };
                };
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
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
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                                } else
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
                    } else
                    {
                        ResolutionsList.SelectedIndex = ResolutionsList.Items.Count - 1;
                    }
                }
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                } else
                {
                    ReShade.IsChecked = false;
                }
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Events
        // After the Page has loaded
        private async void Options_Loaded(object sender, RoutedEventArgs e)
        {
            GetDirectory();
            await FindSupportedResolutions();
            await ReadConfigFiles();
        }

        // Save settings when button Save is clicked
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
