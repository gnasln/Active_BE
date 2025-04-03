using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Identity
{
    public record UpdateUser
    {
        public string? UserName { get; init; }
        public DateTime? Birthday { get; init; } = DateTime.MinValue;
        [EmailAddress]
        public string? Email { get; init; }
        [Phone]
        public string? Phone { get; init; }
        public string? Address { get; init; }
        public string? Status {get; init;}
        public string? FullName{get; init;}
        public DateTime? ActivationDate {get; init;}
    }
}