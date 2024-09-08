using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Email
{
    public class EmailMessage
    {
        private EmailMessage() { }

        public string ToAddress { get; private set; }

        public string Body { get; private set; }

        public string Subject { get; private set; }

        public List<EmailAttachment> Attachments { get; private set; }

        public EmailMessage(string toAddress, string body, string subject)
        {
            ToAddress = toAddress;
            Body = body;
            Subject = subject;
            Attachments = new List<EmailAttachment>();
        }

        public  EmailMessage(string toAddress, string body, string subject, List<EmailAttachment> attachments) : this(toAddress, body, subject)
        {
            ToAddress = toAddress;
            Body = body;
            Subject = subject;
            Attachments = attachments;
        }
    }
}
