using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace Assassins_Creed_Remastered_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetDirectory();
            GC.Collect();
        }

        // Global
        private string path = "";

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
            Environment.Exit(0);
        }

        // Go back 1 page in Frame when this button is clicked
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (PageViewer.NavigationService.CanGoBack)
            {
                PageViewer.GoBack();
            }
        }

        // Opens uMod and the Game and then waits for Game to be closed and then closes the uMod
        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process uMod = new Process();
                Process Game = new Process();
                uMod.StartInfo.WorkingDirectory = path + @"\uMod";
                uMod.StartInfo.FileName = "uMod.exe";
                Game.StartInfo.WorkingDirectory = path;
                Game.StartInfo.FileName = "AssassinsCreed_Dx9.exe";
                uMod.Start();
                Game.Start();
                Game.WaitForExit();
                uMod.CloseMainWindow();
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            };
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Navigate(new Uri("Pages/Credits.xaml", UriKind.Relative));
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Navigate(new Uri("Pages/Options.xaml", UriKind.Relative));
        }
    }
}
