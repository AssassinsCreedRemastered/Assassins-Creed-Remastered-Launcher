using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace ACRemasteredLauncher.Pages
{
    /// <summary>
    /// Interaction logic for Options_Page.xaml
    /// </summary>
    public partial class Options_Page : Page
    {
        public Options_Page()
        {
            InitializeComponent();
        }

        List<Resolutions> compatibleResolutions = new List<Resolutions>();

        public void Options_Loaded(object sender, RoutedEventArgs e)
        {
            FindSupportedResolutions();
        }

        public void FindSupportedResolutions()
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public void Test()
        {
            try
            {
                foreach (var resolution in compatibleResolutions)
                {
                    Console.WriteLine(resolution.Resolution);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
