using System.Collections.Generic;
using System.Xml;

namespace AutoChecker
{
    static class ConfigReader
    {
        // Uri for the website to parse
        public static string Uri { get; private set; } = string.Empty;
        // Filter (img URL for current need)
        public static string FilterVal { get; private set; } = string.Empty;
        // Email server
        public static string EmailServer { get; private set; } = string.Empty;
        // Email account list
        public static List<Account> AccountList { get; private set; } = new List<Account>();

        /// <summary>
        /// Load config from XML 
        /// </summary>
        /// <param name="configPath">XML file path</param>
        public static void LoadConfig(string configPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configPath);
            Uri = xmlDoc.SelectSingleNode("Root/Website").Attributes["uri"].Value.Trim();
            FilterVal = xmlDoc.SelectSingleNode("Root/Website/Filter").Attributes["value"].Value.Trim();
            var emailNode = xmlDoc.SelectSingleNode("Root/EmailAuthentication");
            EmailServer = emailNode.SelectSingleNode("Account").Attributes["server"].Value.Trim();
            foreach (XmlNode accountNode in emailNode.ChildNodes)
            {
                AccountList.Add
                (
                    new Account
                    (
                        accountNode.SelectSingleNode("Username").InnerText.Trim(),
                        accountNode.SelectSingleNode("Password").InnerText.Trim()
                    )
                );
            }
        }
    }

    class Account
    {
        // Email username
        public string Username { get; private set; } = string.Empty;
        // Email password
        public string Password { get; private set; } = string.Empty;

        public Account(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
