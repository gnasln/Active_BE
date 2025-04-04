namespace Bff.Application.Contracts.Persistence
{
    public interface IMailServices
    {
        Task SendEmailAsync(string email, string name, string otp);
    }
}