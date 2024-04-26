using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailKit_Template_Mail
{
    public class SendEmailBySMTPInput
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public List<string> Recipient { get; set; }
    }
}
