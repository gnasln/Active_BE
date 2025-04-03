namespace Bff.Identity
{
    public record NewUserFromOwnerTenant
    {
        public required string UserName { get; init; }
        public required string Password { get; init; }
        public required string Repassword { get; init; }
        public required Guid TenantId { get; init; }
    }
}