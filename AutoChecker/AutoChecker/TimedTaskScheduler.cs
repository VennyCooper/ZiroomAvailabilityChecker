using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AutoChecker
{
    class TimedTaskScheduler
    {
        private HtmlParser _htmlParser = null;
        private EmailSender _emailSender = null;
        private int _iteration = 0;

        public TimedTaskScheduler()
        {
            _htmlParser = new HtmlParser(ConfigReader.Uri);
            _emailSender = new EmailSender();
        }

        public void RunTimedTask()
        {
            Timer taskTimer = new Timer();
            taskTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            taskTimer.Interval = ConfigReader.TimerInterval;
            taskTimer.Enabled = true;
            Logger.WriteLine($"[Done] Setup timer: Interval={taskTimer.Interval/1000}s");
            taskTimer.Start();
            while (true) { }
        }

        private void OnTimeEvent(object obj, ElapsedEventArgs e)
        {
            Logger.WriteLine($"Iteration-{_iteration++}");
            _htmlParser.RefreshPage();
            var validRooms = _htmlParser.GetValidRooms();
            if (validRooms.Length > 0)
            {
                Logger.WriteLine($"++ [Found] {validRooms.Length} available rooms");
                _emailSender.ReceiveValidRooms(validRooms);
                _emailSender.RunEmailSender();
            }
            else
            {
                Logger.WriteLine("-- No available room");
            }
        }
    }
}
