using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Reflection;

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

        public void ReceiveValidRooms(IEnumerable<RoomInfo> validRooms)
        {
            this.validRooms = validRooms;
        }

        public void RunEmailSender()
        {
            SendEmail();
        }

        private void SendEmail()
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = ConfigReader.EmailServer;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(
                ConfigReader.Username, 
                ConfigReader.Password
                );

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigReader.Username);
            foreach (string recipient in ConfigReader.RecipientList)
            {
                mail.To.Add(new MailAddress(recipient));
            }
            mail.Subject = $"Room Availability Update - {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}";
            mail.BodyEncoding = Encoding.UTF8;
            mail.Body = GenerateMailBody();
            mail.IsBodyHtml = true;
            client.Send(mail);
        }

        private string GenerateMailBody()
        {
            StringBuilder mailBody = new StringBuilder();
            // Append prefix
            mailBody.Append("<!DOCTYPE html><html><body>");
            mailBody.Append("<style>th,td,h2{font-family:Arial,Helvetica,sans-serif;}</style>");
            // Append description paragraph
            mailBody.Append($"<h2>Found room updates at {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}</h2>");
            mailBody.Append("<table style=\"width:100%\" border=\"1\">");
            // Append table header row
            mailBody.Append($"<tr>{GetRoomInfoPropertyNames()}</tr>");
            // Append table room rows
            foreach (RoomInfo room in validRooms)
            {
                mailBody.Append($"<tr>{ParseRoomInfoPropertyValues(room)}</tr>");
            }
            // Append suffix
            mailBody.Append("</table></body></html>");
            return mailBody.ToString();
        }

        private string GetRoomInfoPropertyNames()
        {
            return propertyInfos
                .Where(x => !"Img".Equals(x.Name))
                .Select(x => $"<th>{x.Name}</th>")
                .Aggregate((a, b) => $"{a}{b}");
        }

        private string ParseRoomInfoPropertyValues(RoomInfo room)
        {
            List<string> valueList = new List<string>();
            foreach (PropertyInfo pInfo in propertyInfos)
            {
                if (pInfo.PropertyType == typeof(List<string>))
                {
                    valueList.Add((pInfo.GetValue(room) as List<string>).Aggregate((a, b) => $"{a} | {b}"));
                }
                else
                {
                    if (!"Img".Equals(pInfo.Name))
                        valueList.Add(pInfo.GetValue(room).ToString());
                }
            }
            return valueList.Select(x => $"<td>{x}</td>").Aggregate((a, b) => $"{a}{b}");
        }
    }
}
