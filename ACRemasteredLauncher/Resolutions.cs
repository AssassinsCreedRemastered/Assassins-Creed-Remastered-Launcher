using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRemasteredLauncher
{
    class Resolutions
    {
        public string Resolution { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public List<int> RefreshRate = new List<int>();
    }
}
