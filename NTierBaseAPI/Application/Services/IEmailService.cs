using Application.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task SendEmailAsync(EmailMessage email);
    }
}
