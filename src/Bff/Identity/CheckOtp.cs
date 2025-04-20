using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Identity
{
    public record CheckOtp
    {
        public required string OTP {get; init;}
        public required string Email {get; init;}
    }
}