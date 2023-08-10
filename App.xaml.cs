using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Assassins_Creed_Remastered_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DiscordRpcClient client;
        private Stopwatch stopwatch = new Stopwatch();
        private Timer timer;
        private string timeElapsed;
        private string path = "";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            foreach (var s in e.Args)
            {
                if (s == "--skiplauncher")
                {
                    GetDirectory();
                    IdleRichPresence();
                    StartGame();
                }
            }
        }
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
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async void StartGame()
        {
            try
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
                InGameRichPresence();
                Game.WaitForExit();
                uMod.CloseMainWindow();
                IdleRichPresence();
                stopwatch.Stop();
                stopwatch.Reset();
                await Task.Delay(10);
                // Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            };
        }

        // Discord Rich Presence when in Launcher
        private void IdleRichPresence()
        {
            client = new DiscordRpcClient("");
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
    }
}
