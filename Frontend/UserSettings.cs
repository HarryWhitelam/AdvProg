using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frontend
{
    /// <summary>
    /// Class <c>UserSettings</c> is used to save and read user-preferences from an XML file. 
    /// </summary>
    public class UserSettings
    {
        public List<string> settings { get; set; }

        public UserSettings() { }
        
        /// <summary>
        /// Method <c>Save</c> serialises and saves any user settings in the XML file. 
        /// </summary>
        /// <param name="filename"><c>filename</c> provides a path to the XML file</param>
        public void Save(string filename)
        {
            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(UserSettings));
                xmls.Serialize(streamWriter, this);
            }
        }

        /// <summary>
        /// Method <c>Read</c> reads and deserialises the data from the XML file
        /// </summary>
        /// <param name="filename"><c>filename</c> provides a path to the XML file</param>
        /// <returns>An instance of UserSettings with the user's preferences</returns>
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
