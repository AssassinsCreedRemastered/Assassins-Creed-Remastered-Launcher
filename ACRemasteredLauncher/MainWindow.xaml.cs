using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
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
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ACRemasteredLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool FirstRun = false;
        bool GraphicsModEnabled = false;
        string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string InstallationDirectory = Directory.GetCurrentDirectory();
        public MainWindow()
        {
            InitializeComponent();
            CreateShortcut();
            CheckIfFoldersExist();
            if (FirstRun)
            {
                FirstRunSetup();
            }
            else
            {
                using (StreamReader sr = new StreamReader(AppData + @"\Ubisoft\Assassin's Creed\Launcher.config"))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] split = line.Split('=');
                        if (split[0].StartsWith("InstallationDirectory"))
                        {
                            InstallationDirectory = split[1];
                            break;
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            GC.Collect();
        }

        private void CheckIfFoldersExist()
        {
            if (!System.IO.Directory.Exists(AppData + @"\Ubisoft\Assassin's Creed"))
            {
                FirstRun = true;
                System.IO.Directory.CreateDirectory(AppData + @"\Ubisoft\Assassin's Creed");
                if (!System.IO.File.Exists(AppData + @"\Ubisoft\Assassin's Creed\Launcher.config"))
                {
                    using (StreamWriter sw = new StreamWriter(AppData + @"\Ubisoft\Assassin's Creed\Launcher.config"))
                    {
                        sw.WriteLine("InstallationDirectory=" + InstallationDirectory);
                    }
                }
            }
            else
            {
                if (!System.IO.File.Exists(AppData + @"\Ubisoft\Assassin's Creed\Launcher.config"))
                {
                    FirstRun = true;
                    using (StreamWriter sw = new StreamWriter(AppData + @"\Ubisoft\Assassin's Creed\Launcher.config"))
                    {
                        sw.WriteLine("InstallationDirectory=" + InstallationDirectory);
                    }
                } else
                {
                    using (StreamReader sr = new StreamReader(AppData + @"\Ubisoft\Assassin's Creed\Launcher.config"))
                    {
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            string[] split = line.Split('=');
                            if (split[0].StartsWith("InstallationDirectory"))
                            {
                                InstallationDirectory = split[1];
                                break;
                            }
                            line = sr.ReadLine();
                        }
                    }
                }
            }
            if (!System.IO.Directory.Exists(AppData + @"\uMod\"))
            {
                FirstRun = true;
                System.IO.Directory.CreateDirectory(AppData + @"\uMod");
                if (!System.IO.File.Exists(AppData + @"\uMod\uMod_DX9.txt"))
                {
                    string ExecutableDirectory = InstallationDirectory + @"\AssassinsCreed_Dx9.exe";
                    char[] array = ExecutableDirectory.ToCharArray();
                    List<char> charList = new List<char>();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (i == 0)
                        {
                            charList.Add(array[i]);
                        }
                        else
                        {
                            charList.Add('\0');
                            charList.Add(array[i]);
                        }
                    }
                    charList.Add('\0');
                    char[] charArray = charList.ToArray();
                    string ExecutablePath = new string(charArray);
                    using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\uMod\uMod_DX9.txt"))
                    {
                        sw.Write(ExecutablePath);
                    }
                }
            } else
            {
                if (!System.IO.File.Exists(AppData + @"\uMod\uMod_DX9.txt"))
                {
                    FirstRun = true;
                    string ExecutableDirectory = InstallationDirectory + @"\AssassinsCreed_Dx9.exe";
                    char[] array = ExecutableDirectory.ToCharArray();
                    List<char> charList = new List<char>();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (i == 0)
                        {
                            charList.Add(array[i]);
                        }
                        else
                        {
                            charList.Add('\0');
                            charList.Add(array[i]);
                        }
                    }
                    char[] charArray = charList.ToArray();
                    string ExecutablePath = new string(charArray);
                    //ExecutablePath = ExecutablePath.Replace("਍", "");
                    using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\uMod\uMod_DX9.txt"))
                    {
                        sw.Write(ExecutablePath);
                    }
                }
            }
            GC.Collect();
        }

        private void CreateShortcut()
        {
            if (!System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Assassin's Creed Remastered.lnk"))
            {
                MessageBoxResult result = MessageBox.Show("Do you want to create shortcut?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    WshShell shell = new WshShell();
                    string SearchLocation = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                    string ShortcutLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Assassin's Creed Remastered.lnk";
                    IWshShortcut Shortcut = shell.CreateShortcut(ShortcutLocation);
                    Shortcut.Description = "Shortcut for Assassin's Creed Remastered";
                    Shortcut.IconLocation = InstallationDirectory + @"\icon.ico";
                    Shortcut.TargetPath = InstallationDirectory + @"\AssassinsCreedRemasteredLauncher.exe";
                    Shortcut.Save();
                    System.IO.File.Copy(ShortcutLocation, SearchLocation + @"\Assassin's Creed Remastered.lnk");
                }
            }
            GC.Collect();
        }

        private void FirstRunSetup()
        {
            using (StreamWriter sw = new StreamWriter(InstallationDirectory + @"\Mods\uMod\templates\ac1.txt"))
            {
                sw.Write("SaveAllTextures:0\n");
                sw.Write("SaveSingleTexture:0\n");
                sw.Write("FontColour:255,0,0\n");
                sw.Write("TextureColour:0,255,0\n");
                sw.Write("Add_true:" + InstallationDirectory + @"\Mods\AC1 Overhaul\Assassin's Creed Overhaul 2016 Full Version.tpf" + "\n");
            }
            string SaveFile = InstallationDirectory + @"\AssassinsCreed_Dx9.exe" + "|" + InstallationDirectory + @"\Mods\uMod\templates\ac1.txt";
            char[] array = SaveFile.ToCharArray();
            List<char> charList = new List<char>();
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    charList.Add(array[i]);
                }
                else
                {
                    charList.Add('\0');
                    charList.Add(array[i]);
                }
            }
            charList.Add('\0');
            char[] charArray = charList.ToArray();
            string SaveFilePATH = new string(charArray);
            using (StreamWriter sw = new StreamWriter(InstallationDirectory + @"\Mods\uMod\uMod_SaveFiles.txt"))
            {
                sw.Write(SaveFilePATH);
            }
            GC.Collect();
        }

        //Events
        private void Window_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Process uMod = new Process();
            Process Game = new Process();
            uMod.StartInfo.WorkingDirectory = InstallationDirectory + @"\Mods\uMod\";
            uMod.StartInfo.FileName = "uMod.exe";
            Game.StartInfo.WorkingDirectory = InstallationDirectory;
            Game.StartInfo.FileName = "AssassinsCreed_Dx9.exe";
            if (GraphicsModEnabled || FirstRun)
            {
                uMod.Start();
            }
            Game.Start();
            Game.WaitForExit();
            if (GraphicsModEnabled || FirstRun)
            {
                uMod.CloseMainWindow();
            }
            GC.Collect();
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Options optionsWindow = new Options();
            string GraphicsMod = optionsWindow.GraphicsModSelection.SelectedItem.ToString();
            bool PS3Icons = false;
            optionsWindow.ShowDialog();
            if (optionsWindow.PS3Icons.IsChecked == true)
            {
                PS3Icons = true;
            }
            if (GraphicsMod == "Original" && PS3Icons == false)
            {
                GraphicsModEnabled = false;
            }
            else if (PS3Icons == true || GraphicsMod != "Original")
            {
                GraphicsModEnabled = true;
            }
            optionsWindow.Close();
            GC.Collect();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            Credits credits = new Credits();
            credits.ShowDialog();
            credits.Close();
            GC.Collect();
        }
    }
}
