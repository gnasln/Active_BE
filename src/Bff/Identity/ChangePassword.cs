using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Identity
{
    public record ChangePassword
    {
        public required string oldPassword { get; set; }
        public required string newPassword {get; set;}
        public required string comfirmedPassword { get; set; }
    }
}