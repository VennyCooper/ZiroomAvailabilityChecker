using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Local XML config path
            string configPath = @"E:\MyRepo\GitHubRepo\PrivateConfigs\ZiroomAvailabilityChecker\Config.xml";
            ConfigReader.LoadConfig(configPath);
            HtmlParser htmlParser = new HtmlParser(ConfigReader.Uri);
            htmlParser.GetRoomInfo();
        }
    }
}
