using Application.Common;
using Application.Common.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Impl
{
    public class EmailService : IEmailService
    {
        private SmtpConfiguration _smtpConfiguration;

        public EmailService(IOptions<SmtpConfiguration> smtpConfiguration)
        {
            _smtpConfiguration = smtpConfiguration.Value;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            using var client = new SmtpClient();

            var builder = new BodyBuilder { HtmlBody = emailMessage.Body };

            if (emailMessage.Attachments.Count > 0)
                foreach (var attachment in emailMessage.Attachments)
                    builder.Attachments.Add(attachment.Name, attachment.Value);

            var email = new MimeMessage
            {
                Subject = emailMessage.Subject,
                Body = builder.ToMessageBody()
            };

            email.From.Add(new MailboxAddress(_smtpConfiguration.SenderName, _smtpConfiguration.SenderEmail));
            email.To.Add(new MailboxAddress(emailMessage.ToAddress.Split("@")[0], emailMessage.ToAddress));

            try
            {
                await client.ConnectAsync(_smtpConfiguration.Server, _smtpConfiguration.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.CheckCertificateRevocation = false;
                await client.AuthenticateAsync(_smtpConfiguration.Username, _smtpConfiguration.Password);

                await client.SendAsync(email);
            }
            catch
            {
                await client.DisconnectAsync(true);
                client.Dispose();

                throw;
            }
        }
    }
}
