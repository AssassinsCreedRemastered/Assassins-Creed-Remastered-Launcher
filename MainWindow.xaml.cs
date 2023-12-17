using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using File = System.IO.File;
using DiscordRPC;
using System.Windows.Threading;
using System.Timers;
using Microsoft.VisualBasic.Logging;

namespace Assassins_Creed_Remastered_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiscordRpcClient client;
        private Stopwatch stopwatch = new Stopwatch();
        private Timer timer;
        private string timeElapsed;

        public MainWindow()
        {
            InitializeComponent();
            GetDirectory();
            IdleRichPresence();
            GC.Collect();
        }

        // Global
        private string path = "";

        // Cache for all of the pages
        private Dictionary<string, Page> pageCache = new Dictionary<string, Page>();

        // Functions
        // Grabbing path where AC is installed
        private async void GetDirectory()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ubisoft\Assassin's Creed\Path.txt"))
                {
                    path = sr.ReadLine();
                }
                if (!Directory.Exists(path + @"\Mods\Custom uMods\"))
                {
                    Directory.CreateDirectory(path + @"\Mods\Custom uMods\");
                }
                GC.Collect();
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Discord Rich Presence when in Launcher
        private void IdleRichPresence()
        {
            client = new DiscordRpcClient("1133864004549361686");
            client.Initialize();
            client.SetPresence(new RichPresence()
            {
                State = "Idle",
                Assets = new Assets()
                {
                    LargeImageKey = "icon1",
                    SmallImageKey = "icon1"
                }
            });
        }

        private void InGameRichPresence()
        {
            timer = new Timer();
            timer.Interval = 10;
            timer.Elapsed += timerElapsed;
            timer.Start();
            stopwatch.Start();
        }

        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateRichPresence();
        }

        // Update Discord Rich Presence
        private void UpdateRichPresence()
        {
            try
            {
                timeElapsedUpdate();
                client.SetPresence(new RichPresence()
                {
                    State = "Playing for " + timeElapsed,
                    Assets = new Assets()
                    {
                        LargeImageKey = "icon1",
                        SmallImageKey = "icon1"
                    }
                });
                if (timer.Interval == 10)
                {
                    timer.Interval = 60000;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        // Format time
        private void timeElapsedUpdate()
        {
            if (stopwatch.Elapsed.Days > 0)
            {
                timeElapsed = stopwatch.Elapsed.Days + " days";
            } 
            else if (stopwatch.Elapsed.Hours > 0)
            {
                timeElapsed = stopwatch.Elapsed.Hours + " hours";
            } 
            else if (stopwatch.Elapsed.Minutes >= 0)
            {
                timeElapsed = stopwatch.Elapsed.Minutes + " minutes";
            }
        }

        // Used to change pages and cache them
        private async Task NavigateToPage(string PageName)
        {
            Console.WriteLine($"Trying to navigate to {PageName}");
            switch (PageName)
            {
                case "Credits":

                    if (!pageCache.ContainsKey(PageName))
                    {

                        Console.WriteLine("Page is not cached. Loading it and caching it for future use.");
                        Pages.Credits page = new Pages.Credits();
                        pageCache[PageName] = page;
                        PageViewer.Content = pageCache[PageName];
                    }
                    else
                    {
                        Console.WriteLine("Page is already cached. Loading it");
                        PageViewer.Content = pageCache[PageName];
                    }
                    break;
                case "Options":
                    if (!pageCache.ContainsKey(PageName))
                    {
                        Console.WriteLine("Page is not cached. Loading it and caching it for future use.");
                        Pages.Options page = new Pages.Options();
                        pageCache[PageName] = page;
                        PageViewer.Content = page;
                    }
                    else
                    {
                        Console.WriteLine("Page is already cached. Loading it");
                        PageViewer.Content = pageCache[PageName];
                    }
                    break;
                case "Mods":
                    if (!pageCache.ContainsKey(PageName))
                    {
                        Console.WriteLine("Page is not cached. Loading it and caching it for future use.");
                        Pages.Mods page = new Pages.Mods();
                        pageCache[PageName] = page;
                        PageViewer.Content = page;
                    }
                    else
                    {
                        Console.WriteLine("Page is already cached. Loading it");
                        PageViewer.Content = pageCache[PageName];
                    }
                    break;
                default:
                    if (!pageCache.ContainsKey(PageName))
                    {
                        Console.WriteLine("Page is not cached. Loading it and caching it for future use.");
                        Pages.Default page = new Pages.Default();
                        pageCache[PageName] = page;
                        PageViewer.Content = page;
                    }
                    else
                    {
                        Console.WriteLine("Page is already cached. Loading it");
                        PageViewer.Content = pageCache[PageName];
                    }
                    break;
            }
            GC.Collect();
            await Task.Delay(1);
        }

        // Events
        // Used for dragging the window
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        
        // Exit the launcher when this button is pressed
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            stopwatch.Reset();
            client.Dispose();
            GC.Collect();
            Environment.Exit(0);
        }

        // Opens uMod and the Game and then waits for Game to be closed and then closes the uMod
        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.uModStatus)
                {
                    Process uMod = new Process();
                    Process Game = new Process();
                    uMod.StartInfo.WorkingDirectory = path + @"\uMod";
                    uMod.StartInfo.FileName = "uMod.exe";
                    uMod.StartInfo.UseShellExecute = true;
                    Game.StartInfo.WorkingDirectory = path;
                    Game.StartInfo.FileName = "AssassinsCreed_Dx9.exe";
                    Game.StartInfo.UseShellExecute = true;
                    uMod.Start();
                    Game.Start();
                    Game.ProcessorAffinity = new IntPtr(0xFF);
                    Game.WaitForExit();
                    await Task.Delay(500);
                    uMod.CloseMainWindow();
                    InGameRichPresence();
                    Game.WaitForExit();
                    await Task.Delay(500);
                    uMod.CloseMainWindow();
                    IdleRichPresence();
                    stopwatch.Stop();
                    stopwatch.Reset();
                }
                else
                {
                    Process Game = new Process();
                    Game.StartInfo.WorkingDirectory = path;
                    Game.StartInfo.FileName = "AssassinsCreed_Dx9.exe";
                    Game.StartInfo.UseShellExecute = true;
                    Game.Start();
                    Game.ProcessorAffinity = new IntPtr(0xFF);
                    InGameRichPresence();
                    Game.WaitForExit();
                    IdleRichPresence();
                    stopwatch.Stop();
                    stopwatch.Reset();
                }
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            };
        }

        // Donate button
        private async void Donate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Donations are and will always be optional.\nEverything made by me on my own in my spare time will always be free and fully open source.\nDon't donate unless you can really afford it and don't donate your parents money without them knowing.\nDonations are NOT refundable.");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://ko-fi.com/shazzaam/",
                    UseShellExecute = true,
                });
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async void Credits_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToPage("Credits");
            GC.Collect();
            await Task.Delay(1);
            //PageViewer.Navigate(new Uri("Pages/Credits.xaml", UriKind.Relative));
        }

        private async void Options_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToPage("Options");
            GC.Collect();
            await Task.Delay(1);
            //PageViewer.Navigate(new Uri("Pages/Options.xaml", UriKind.Relative));
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentVersion = "";
                string newestVersion = "";
                using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Assassins_Creed_Remastered_Launcher.Version.txt")))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        if (line != "")
                        {
                            Console.WriteLine("Current Version: " + line);
                            currentVersion = line;
                        }
                        line = sr.ReadLine();
                    }
                }
                HttpWebRequest SourceText = (HttpWebRequest)HttpWebRequest.Create("https://raw.githubusercontent.com/AssassinsCreedRemastered/Assassins-Creed-Remastered-Launcher/Version/Version.txt");
                SourceText.UserAgent = "Mozilla/5.0";
                var response = SourceText.GetResponse();
                var content = response.GetResponseStream();
                using (var reader = new StreamReader(content))
                {
                    string fileContent = reader.ReadToEnd();
                    string[] lines = fileContent.Split(new char[] { '\n' });
                    foreach (string line in lines)
                    {
                        if (line != "")
                        {
                            Console.WriteLine(line);
                            newestVersion = line;
                        }
                    }
                }
                if (currentVersion == newestVersion)
                {
                    MessageBox.Show("Newest version is installed.");
                    GC.Collect();
                    await Task.Delay(1);
                    return;
                } else
                {
                    MessageBoxResult result = MessageBox.Show("New version of the launcher found. Do you want to update?", "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process updater = new Process();
                        updater.StartInfo.FileName = "Assassins Creed Remastered Launcher Updater.exe";
                        updater.StartInfo.WorkingDirectory = path;
                        updater.StartInfo.UseShellExecute = true;
                        updater.Start();
                        Environment.Exit(0);
                    } else
                    {
                        GC.Collect();
                        await Task.Delay(1);
                        return;
                    }
                }
                GC.Collect();
                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async void uMod_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToPage("Mods");
            GC.Collect();
            await Task.Delay(1);
            //PageViewer.Navigate(new Uri("Pages/Mods.xaml", UriKind.Relative));
        }
    }
}
