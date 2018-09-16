using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoChecker
{
    class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">1. config path, 2. log folder path</param>
        static void Main(string[] args)
        {
            string configPath = @"E:\MyRepo\GitHubRepo\PrivateConfigs\ZiroomAvailabilityChecker\Config.xml";
            string logDir = @"C:\Users\Wenqi\Desktop\ZiroomChecker\Log";
            Logger.SetLogger(logDir);
            ConfigReader.LoadConfig(configPath);
            TimedTaskScheduler timedTask = new TimedTaskScheduler();
            timedTask.RunTimedTask();
            //HtmlParser htmlParser = new HtmlParser(ConfigReader.Uri);
            //EmailSender emailSender = new EmailSender();
            //// Timed task
            //htmlParser.RefreshPage();
            //var validRooms = htmlParser.GetValidRooms();
            //if (validRooms.Length > 0)
            //{
            //    Logger.WriteLine($"++ [Found] {validRooms.Length} available rooms");
            //    emailSender.ReceiveValidRooms(validRooms);
            //    emailSender.RunEmailSender();
            //}
            //else
            //{
            //    Logger.WriteLine("-- No available room");
            //}
        }
    }
}
