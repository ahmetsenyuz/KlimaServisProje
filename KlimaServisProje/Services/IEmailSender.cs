using KlimaServisProje.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KlimaServisProje.Services
{
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message);
    }
}
