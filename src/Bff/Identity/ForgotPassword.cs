using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Identity
{
    public record ForgotPassword
    {
        public required string UserName {get; init;}
        public required string new_password {get; init;}
        public required string confirmed_password {get; init;}
    }
}