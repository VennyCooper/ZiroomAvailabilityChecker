using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AutoChecker
{
    static class Logger
    {
        private static string _logPath = string.Empty;

        public static void SetLogger(string logDir)
        {
            try
            {
                Directory.CreateDirectory(logDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
            _logPath = Path.Combine(logDir, $"Log_{DateTime.Now.ToString("yyyyddMM_HH_mm_ss")}.txt");
        }

        public static void WriteLine(string msg)
        {
            msg = $"{DateTime.Now.ToString("yyyyddMM HH:mm:ss")}|\t{msg}";
            Console.WriteLine(msg);
            File.AppendAllLines(_logPath, new List<string>() { $"{msg}" });
        }
    }
}
