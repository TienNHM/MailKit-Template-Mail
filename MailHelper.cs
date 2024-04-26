using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailKit_Template_Mail
{
    public static class MailHelper
    {
        private static string _emailFrom = Properties.Settings.Default.emailFrom;
        private static string _emailHost = Properties.Settings.Default.emailHost;
        private static int _emailPort = Properties.Settings.Default.emailPort;
        private static string _emailUsername = Properties.Settings.Default.emailUsername;
        private static string _emailPassword = Properties.Settings.Default.emailPassword;
        private static SecureSocketOptions _secureSocketOptions = SecureSocketOptions.Auto;

        public static async Task<SendEmailBySMTPOutput> SendEmailBySMTPAsync(SendEmailBySMTPInput input)
        {
            var result = new SendEmailBySMTPOutput();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailFrom));
            foreach (var item in input.Recipient)
            {
                emailMessage.To.Add(MailboxAddress.Parse(item));
            }

            emailMessage.Subject = input.Title;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = input.Content,
                //ContentTransferEncoding = ContentEncoding.Base64
            };

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_emailHost, _emailPort, _secureSocketOptions);
                    await client.AuthenticateAsync(_emailUsername, _emailPassword);

                    await client.SendAsync(emailMessage);
                    result.IsSuccess = true;

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex) //todo add another try to send email
            {
                var e = ex;
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                throw;
            }

            return result;
        }

        public static SendEmailBySMTPOutput SendEmailBySMTP(SendEmailBySMTPInput input)
        {
            var result = new SendEmailBySMTPOutput();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailFrom));
            foreach (var item in input.Recipient)
            {
                emailMessage.To.Add(MailboxAddress.Parse(item));
            }

            emailMessage.Subject = input.Title;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = input.Content,
                //ContentTransferEncoding = ContentEncoding.Base64
            };

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(_emailHost, _emailPort, _secureSocketOptions);
                    client.Authenticate(_emailUsername, _emailPassword);
                    client.Send(emailMessage);
                    result.IsSuccess = true;
                    client.Disconnect(true);
                }
            }
            catch (Exception ex) //todo add another try to send email
            {
                var e = ex;
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                throw;
            }

            return result;
        }
    }
}
