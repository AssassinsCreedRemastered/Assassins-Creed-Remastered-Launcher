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

        public Options()
        {
            InitializeComponent();
            GC.Collect();
        }


        //Events
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
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

        }

        private void GraphicsModSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ReShadePreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PS3Icons_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
