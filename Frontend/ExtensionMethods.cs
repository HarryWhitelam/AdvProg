using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static long Lines(System.Windows.Controls.RichTextBox rtb)
        {
            string str = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
            long count = 1;
            int position = 0;
            while ((position = str.IndexOf('\n', position)) != -1)
            {
                count++;
                position++;
            }
            return count;
        }
    }
}
