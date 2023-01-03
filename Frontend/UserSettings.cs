using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frontend
{
    class UserSettings
    {
        public List<string> settings { get; set; }

        public UserSettings(List<string> settingsList)
        {
            this.settings = settingsList;
        }
        
        public void Save(string filename)
        {
            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(UserSettings));
                xmls.Serialize(streamWriter, this);
            }
        }

        public static UserSettings Read(string filename)
        {
            using (StreamReader streamReader = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(UserSettings));
                return xmls.Deserialize(streamReader) as UserSettings;
            }
        }
    }
}
