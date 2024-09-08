using Application.Common.Email;
using MailKit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DevImpl
{
    public class DevEmailService : IEmailService
    {
        private ILogger<DevEmailService> _logger;

        public DevEmailService(ILogger<DevEmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage email)
        {
            await Task.Delay(10);
            _logger.LogInformation($"Email was sent to: [{email.ToAddress}]. Body: {email.Body}");
        }
    }
}
