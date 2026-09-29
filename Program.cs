using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MailKit_Template_Mail
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MailKit-Template-Mail <recipient-email> [<recipient-email> ...]");
                return;
            }

            var result = TestSendEmailBySMTP(new List<string>(args));
            if (result.IsSuccess)
            {
                Console.WriteLine("Send email success");
            }
            else
            {
                Console.WriteLine("Send email fail");
            }
        }

        public static SendEmailBySMTPOutput TestSendEmailBySMTP(List<string> listEmail)
        {
            var otp = GenerateOtp();
            var emailTitle = "Test OTP";
            var emailContent = "Mã OTP xác minh tài khoản";

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SendOtpToEmail.html");
            var content = File.ReadAllText(templatePath, encoding: Encoding.UTF8);
            content = content.Replace("@EmailTitle", WebUtility.HtmlEncode(emailTitle));
            content = content.Replace("@EmailContent", WebUtility.HtmlEncode(emailContent));
            content = content.Replace("@OTP", otp);

            var input = new SendEmailBySMTPInput()
            {
                Title = emailTitle,
                Content = content,
                Recipient = listEmail,
            };
            var output = MailHelper.SendEmailBySMTP(input);

            return output;
        }

        // System.Random is predictable; OTPs must come from a cryptographically secure generator
        private static string GenerateOtp()
        {
            var bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                uint value;
                // Rejection sampling avoids modulo bias
                do
                {
                    rng.GetBytes(bytes);
                    value = BitConverter.ToUInt32(bytes, 0);
                } while (value >= uint.MaxValue - (uint.MaxValue % 900000));

                return (100000 + value % 900000).ToString();
            }
        }
    }
}
