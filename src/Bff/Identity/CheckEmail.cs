using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Identity
{
    public record CheckEmail
    {
        public required string Email {get; init;}
    }
}