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

namespace ACRemasteredLauncher
{
    /// <summary>
    /// Interaction logic for Credits_Page.xaml
    /// </summary>
    public partial class Credits_Page : Page
    {
        public Credits_Page()
        {
            InitializeComponent();
        }

        private void OpenTexModButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://code.google.com/archive/p/texmod/");
        }

        private void OverhaulMod_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.moddb.com/mods/assassins-creed-2014-overhaul");
        }

        private void CryNation_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.patreon.com/ktmx");
        }

        private void RTGI_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.patreon.com/mcflypg");
        }

        private void ReShade_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://reshade.me/");
        }

        private void EaglePatch_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Sergeanur/EaglePatch");
        }

        private void PSButtons_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.nexusmods.com/assassinscreed/mods/10");
        }

        private void _4KIntro_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.nexusmods.com/assassinscreed/mods/22");
        }
    }
}
