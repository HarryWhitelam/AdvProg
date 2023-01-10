using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    class HelpDocMethod
    {
        string opName { get; set; }
        string description { get; set; }
        string example { get; set; }

        public HelpDocMethod(string opName, string description, string example)
        {
            this.opName = opName;
            this.description = description;
            this.example = example;
        }
    }
}
