using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace AutoChecker
{
    class EmailSender
    {

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
            client.Send(mail);
        }

        private string GenerateMailBody()
        {
            string mailBody = string.Empty;



            return mailBody;
        }
    }
}
