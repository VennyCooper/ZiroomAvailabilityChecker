using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace AutoChecker
{
    class EmailSender
    {
        private PropertyInfo[] propertyInfos = null;
        IEnumerable<RoomInfo> validRooms = null;

        public EmailSender()
        {
            propertyInfos = typeof(RoomInfo).GetProperties();
        }

        /// <summary>
        /// Receive valid rooms
        /// </summary>
        /// <param name="validRooms">valid rooms</param>
        public void ReceiveValidRooms(IEnumerable<RoomInfo> validRooms)
        {
            this.validRooms = validRooms;
        }

        /// <summary>
        /// Public entry method to run email sending
        /// </summary>
        public void RunEmailSender()
        {
            SendEmail();
            Logger.WriteLine(">> [Sent] Result email sent to recipient");
        }

        /// <summary>
        /// Private functional method for sending email
        /// </summary>
        private void SendEmail()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.Host = ConfigReader.EmailServer;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigReader.Username, ConfigReader.Password);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigReader.Username);
            foreach (string recipient in ConfigReader.RecipientList)
            {
                mail.To.Add(new MailAddress(recipient));
            }
            mail.Subject = $"Room Availability Update - {DateTime.Now.ToString("yyyy.dd.MM HH:mm:ss")}";
            mail.BodyEncoding = Encoding.UTF8;
            mail.Body = GenerateMailBody();
            mail.IsBodyHtml = true;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.ToString());
                throw e;
            }
            finally
            {
                // Write mail body to file even if there is any exception during email sending
                WriteMailBodyToFile(mail.Body);
            }
            
        }

        /// <summary>
        /// Create email body HTML text
        /// </summary>
        /// <returns>email body HTML text</returns>
        private string GenerateMailBody()
        {
            StringBuilder mailBody = new StringBuilder();
            // Append prefix
            mailBody.Append("<!DOCTYPE html><html><body>");
            mailBody.Append("<style>th,td,h2{font-family:Arial,Helvetica,sans-serif;}</style>");
            // Append description paragraph
            mailBody.Append($"<h2>Found room updates at {DateTime.Now.ToString("yyyy.dd.MM HH:mm:ss")}</h2>");
            mailBody.Append("<table style=\"width:100%\" border=\"1\">");
            // Append table header row
            mailBody.Append($"<tr>{GetRoomInfoHtmlTableHeaders()}</tr>");
            // Append table room rows
            foreach (RoomInfo room in validRooms)
            {
                mailBody.Append($"<tr>{GetRoomInfoHtmlTableRows(room)}</tr>");
            }
            // Append suffix
            mailBody.Append("</table></body></html>");
            return mailBody.ToString();
        }

        /// <summary>
        /// Create room table headers by getting property names of class RoomInfo using reflection
        /// </summary>
        /// <returns></returns>
        private string GetRoomInfoHtmlTableHeaders()
        {
            return propertyInfos
                .Where(x => !"Img".Equals(x.Name))
                .Select(x => $"<th>{x.Name}</th>")
                .Aggregate((a, b) => $"{a}{b}");
        }

        /// <summary>
        /// Create corresponding table row HTML text for a given room
        /// </summary>
        /// <param name="room">a room instance</param>
        /// <returns>table row HTML text for this room</returns>
        private string GetRoomInfoHtmlTableRows(RoomInfo room)
        {
            List<string> valueList = new List<string>();
            foreach (PropertyInfo pInfo in propertyInfos)
            {
                /// Case: property "Styles" of type List<string>
                if (pInfo.PropertyType == typeof(List<string>))
                {
                    valueList.Add((pInfo.GetValue(room) as List<string>)
                        .Aggregate((a, b) => $"{a} | {b}"));
                }
                // Case: other types, double or string
                else
                {
                    if (!"Img".Equals(pInfo.Name))
                        valueList.Add(pInfo.GetValue(room).ToString());
                }
            }
            return valueList.Select(x => $"<td>{x}</td>").Aggregate((a, b) => $"{a}{b}");
        }

        private void WriteMailBodyToFile(string mailBody)
        {
            string resultFile = Path.Combine(ConfigReader.ResultDir, $"Result_{DateTime.Now.ToString("yyyyddMM_HH_mm_ss")}.html");
            File.WriteAllLines(resultFile, new List<string>() { mailBody }, Encoding.UTF8);
        }
    }
}
