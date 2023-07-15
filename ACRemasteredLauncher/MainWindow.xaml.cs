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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ACRemasteredLauncher.Pages;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ACRemasteredLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string path = "";
        public MainWindow()
        {
            InitializeComponent();
            GetDirectory();
            GC.Collect();
        }

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

        //Events
        private void PlayButton_Click(object sender, RoutedEventArgs e)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            };
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Navigate(new Uri("Pages/Options Page.xaml", UriKind.Relative));
        }

        // Exit the program
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        // Open the Credits Page
        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Navigate(new Uri("Pages/Credits Page.xaml", UriKind.Relative));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = PageViewer.NavigationService;
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void PageViewer_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            /*
            switch (e.Uri.ToString())
            {
                case string s when s.EndsWith("Default%20Page.xaml"):
                    break;
                case string s when s.EndsWith("Credits%20Page.xaml"):
                    break;
                case string s when s.EndsWith("Options%20Page.xaml"):
                    break;
                default:
                    break;
            }*/
        }
    }
}
