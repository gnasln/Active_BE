using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bff.Application.Dtos.Mail;

namespace Bff.Application.Contracts.Persistence
{
    public interface IMailServices
    {
        Task<bool> SendMailAsync(MailInfo mailInfo, CancellationToken cancellationToken = default);
    }
}