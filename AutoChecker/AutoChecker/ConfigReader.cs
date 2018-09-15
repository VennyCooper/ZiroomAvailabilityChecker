using System.Collections.Generic;
using System.Xml;

namespace AutoChecker
{
    static class ConfigReader
    {
        // Uri for the page to parse
        public static string Uri { get; private set; } = string.Empty;
        // Filters 
        // Exclusive img URL for current need
        public static string ImgExclusion { get; private set; } = string.Empty;
        // Room orientation (North, South...)
        public static string Orientation { get; private set; } = string.Empty;
        // Room area min value
        public static double MinArea { get; private set; } = 0;
        // Room area max value
        public static double MaxArea { get; private set; } = 0;
        // Room styles
        public static List<string> StyleList { get; private set; } = new List<string>();
        // Email server
        public static string EmailServer { get; private set; } = string.Empty;
        // Email username
        public static string Username { get; private set; } = string.Empty;
        // Email password
        public static string Password { get; private set; } = string.Empty;
        // Email recipient
        public static List<string> RecipientList { get; private set; } = new List<string>();

        /// <summary>
        /// Load config from XML 
        /// </summary>
        /// <param name="configPath">XML file path</param>
        public static void LoadConfig(string configPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configPath);
            // Retrieve page URI
            Uri = xmlDoc.SelectSingleNode("Root/Page").Attributes["uri"].Value.Trim();
            var filterNode = xmlDoc.SelectSingleNode("Root/Page/Filter");
            // Retrieve img src URL for exclusion
            ImgExclusion = filterNode.SelectSingleNode("ImgExclusion").Attributes["src"].Value.Trim();
            // Retrieve room orientation
            Orientation = filterNode.SelectSingleNode("Orientation").Attributes["direction"].Value.Trim();
            // Retrieve room area range
            string minAreaStr = filterNode.SelectSingleNode("Area").Attributes["Min"].Value.Trim();
            MinArea = string.IsNullOrEmpty(minAreaStr) ? double.MinValue : double.Parse(minAreaStr);
            string maxAreaStr = filterNode.SelectSingleNode("Area").Attributes["Max"].Value.Trim();
            MaxArea = string.IsNullOrEmpty(maxAreaStr) ? double.MaxValue : double.Parse(maxAreaStr);
            // Retrieve styles
            foreach (XmlNode styleNode in filterNode.SelectSingleNode("Styles").ChildNodes)
            {
                StyleList.Add(styleNode.InnerText.Trim());
            }
            // Retrieve email server and authentication info
            var emailNode = xmlDoc.SelectSingleNode("Root/Email");
            var emailAccountNode = emailNode.SelectSingleNode("Account");
            EmailServer = emailAccountNode.Attributes["server"].Value.Trim();
            Username = emailAccountNode.SelectSingleNode("Username").InnerText.Trim();
            Password = emailAccountNode.SelectSingleNode("Password").InnerText.Trim();
            // Retrieve recipients
            foreach (XmlNode recipientNode in emailNode.SelectSingleNode("Recipients").ChildNodes)
            {
                RecipientList.Add(recipientNode.InnerText.Trim());
            }

        }
    }
}
