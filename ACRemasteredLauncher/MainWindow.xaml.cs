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
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ACRemasteredLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GC.Collect();
        }

        //Events
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Source = new Uri("Pages/Default Page.xaml", UriKind.Relative);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Source = new Uri("Pages/Options Page.xaml", UriKind.Relative);
        }

        // Exit the program
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        // Open the Credits Page
        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            PageViewer.Source = new Uri("Pages/Credits Page.xaml", UriKind.Relative);
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
            Default_Page dp = new Default_Page();
            switch (e.Uri.ToString())
            {
                case string s when s.EndsWith("Default%20Page.xaml"):
                    Console.WriteLine("Default Page");
                    break;
                case string s when s.EndsWith("Credits%20Page.xaml"):
                    Console.WriteLine("Credits Page");
                    //dp.PageImageSource = new BitmapImage(new Uri("../Assets/background.png"));
                    Console.WriteLine(dp.PageImageSource);
                    dp.Test2();
                    break;
                case string s when s.EndsWith("Options%20Page.xaml"):
                    Console.WriteLine("Options Page");
                    dp.Test2();
                    break;
                default:
                    break;
            }
        }
    }
}
