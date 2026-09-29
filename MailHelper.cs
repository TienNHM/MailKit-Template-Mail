using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace MailKit_Template_Mail
{
    public static class MailHelper
    {
        private const string PasswordEnvironmentVariable = "SMTP_PASSWORD";

        private static readonly string _emailFrom = Properties.Settings.Default.emailFrom;
        private static readonly string _emailHost = Properties.Settings.Default.emailHost;
        private static readonly int _emailPort = Properties.Settings.Default.emailPort;
        private static readonly string _emailUsername = Properties.Settings.Default.emailUsername;

        // Port 465 uses implicit TLS; any other port must upgrade with STARTTLS or fail (never fall back to plaintext)
        private static SecureSocketOptions SecureSocketOptions =>
            _emailPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

        public static async Task<SendEmailBySMTPOutput> SendEmailBySMTPAsync(SendEmailBySMTPInput input)
        {
            var result = new SendEmailBySMTPOutput();
            var emailMessage = BuildMessage(input);

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailHost, _emailPort, SecureSocketOptions);
                    await client.AuthenticateAsync(_emailUsername, GetPassword());
                    await client.SendAsync(emailMessage);
                    result.IsSuccess = true;

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex) //todo add another try to send email
            {
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                throw;
            }

            return result;
        }

        public static SendEmailBySMTPOutput SendEmailBySMTP(SendEmailBySMTPInput input)
        {
            var result = new SendEmailBySMTPOutput();
            var emailMessage = BuildMessage(input);

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(_emailHost, _emailPort, SecureSocketOptions);
                    client.Authenticate(_emailUsername, GetPassword());
                    client.Send(emailMessage);
                    result.IsSuccess = true;

                    client.Disconnect(true);
                }
            }
            catch (Exception ex) //todo add another try to send email
            {
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                throw;
            }

            return result;
        }

        private static MimeMessage BuildMessage(SendEmailBySMTPInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.Recipient == null || input.Recipient.Count == 0)
                throw new ArgumentException("At least one recipient is required.", nameof(input));

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
            };

            return emailMessage;
        }

        private static string GetPassword()
        {
            var password = Environment.GetEnvironmentVariable(PasswordEnvironmentVariable);
            if (string.IsNullOrEmpty(password))
                password = Properties.Settings.Default.emailPassword;
            if (string.IsNullOrEmpty(password))
                throw new InvalidOperationException(
                    $"SMTP password is not configured. Set the {PasswordEnvironmentVariable} environment variable.");

            return password;
        }
    }
}
