using System.ComponentModel.DataAnnotations;

namespace Bff.Identity
{
    public record NewUser
    {
        public required string UserName { get; init; }
        public required string Password { get; init; }
        public required string Repassword { get; init; }
        [EmailAddress]
        public string? Email { get; init; }
        [Phone]
        public string? PhoneNumber { get; init; }
        public string? FullName { get; init; }
        public DateTime? ActivationDate { get; init; } = DateTime.Now;
    }
}
