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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ACRemasteredLauncher
{
    /// <summary>
    /// Interaction logic for Default_Page.xaml
    /// </summary>
    public partial class Default_Page : Page
    {
        public string Test { get; set; }
        public ImageSource PageImageSource
        {
            get { return Background.ImageSource; }
            set { Background.ImageSource = value; }
        }
        public Default_Page()
        {
            InitializeComponent();
            Test = "test";
        }

        public void Test2()
        {
            Ispis();
        }

        private void Ispis()
        {
            Console.WriteLine("test21");
        }
    }
}
