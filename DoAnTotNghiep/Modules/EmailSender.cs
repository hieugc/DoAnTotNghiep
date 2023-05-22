using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using static QRCoder.PayloadGenerator;
using System;

namespace DoAnTotNghiep.Modules
{
    public class EmailSender
    {
        private readonly string _email;
        private readonly string _password;

        public EmailSender(string email, string password)
        {
            _email = email;
            _password = password;
        }
        
        public bool SendMail(string to, string Subject, string body, Stream? file, string filename)
        {
            try
            {
                MailMessage message = new MailMessage(this._email, to);
                message.Subject = Subject;
                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
                System.Net.NetworkCredential basicCredential1 =
                    new System.Net.NetworkCredential(this._email, this._password);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;
                if (file != null)
                {
                    message.Attachments.Add(new Attachment(file, filename));
                }
                client.Send(message);
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "Email_send_to_" + to);
                return false;
            }
            return true;
        }
        public void SendMailInThread(string to, string Subject, string body, Stream? file, string filename)
        {
            Thread thread = new Thread(() => this.SendMail(to, Subject, body, file, filename));
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
