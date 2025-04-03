using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Application.Dtos.Mail
{
    public class MailInfo
    {
        public string? recipient {get; set;}
        public string? subject {get; set;}
        public string? body {get; set;}
    }
}