using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Assassins_Creed_Remastered_Launcher.Pages
{
    /// <summary>
    /// Interaction logic for Credits.xaml
    /// </summary>
    public partial class Credits : Page
    {
        public Credits()
        {
            InitializeComponent();
        }

        private void uMod_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://code.google.com/archive/p/texmod/",
                UseShellExecute = true,
            });
        }

        private void OverhaulMod_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.moddb.com/mods/assassins-creed-2014-overhaul",
                UseShellExecute = true,
            });
        }

        private void EaglePatch_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Sergeanur/EaglePatch",
                UseShellExecute = true,
            });
        }

        private void PSButtons_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.nexusmods.com/assassinscreed/mods/10",
                UseShellExecute = true,
            });
        }

        private void ReShade_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://reshade.me/",
                UseShellExecute = true,
            });
        }

        private void ReShadePreset_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://steamcommunity.com/sharedfiles/filedetails/?id=2957930769",
                UseShellExecute = true,
            });
        }

        private void Icon_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.deviantart.com/sony33d/art/Assassin-s-Creed-Icon-Pack-PNG-ICO-555468864",
                UseShellExecute = true,
            });
        }
    }
}
