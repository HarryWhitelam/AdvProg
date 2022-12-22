using AdvProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frontend
{
    public class ThemeArray : ResourceDictionary
    {
        private Uri light;
        private Uri dark;

        public Uri Light
        {
            get { return light; }
            set
            {
                light = value;
                UpdateTheme();
            }
        }

        public Uri Dark
        {
            get { return dark; }
            set
            {
                dark = value;
                UpdateTheme();
            }
        }

        public void UpdateTheme()
        {
            var val = MainWindow.Theme == Theme.Light ? Light : Dark;
            if (val != null && val != base.Source)
                base.Source = val;
        }
    }
}
