using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailKit_Template_Mail
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = TestSendEmailBySMTP();
            if (result.IsSuccess)
            {
                Console.WriteLine("Send email success");
            }
            else
            {
                Console.WriteLine("Send email fail");
            }
        }

        public static SendEmailBySMTPOutput TestSendEmailBySMTP()
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var emailTitle = "Test OTP";
            var emailContent = "Mã OTP xác minh tài khoản";
            var listEmail = new List<string> { "ngotienhoang09@gmail.com" };

            var content = File.ReadAllText("./SendOtpToEmail.html", encoding: Encoding.UTF8);
            content = content.Replace("@EmailTitle", emailTitle);
            content = content.Replace("@EmailContent", emailContent);
            content = content.Replace("@OTP", otp);

            var input = new SendEmailBySMTPInput()
            {
                Title = emailTitle,
                Content = content,
                Recipient =listEmail,
            };
            var output = MailHelper.SendEmailBySMTP(input);

            return output;
        }
    }
}
