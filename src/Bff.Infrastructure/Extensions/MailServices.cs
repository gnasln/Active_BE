using System.Net;
using Microsoft.Extensions.Configuration;
using Bff.Application.Contracts.Persistence;
using Bff.Application.Dtos.Mail;
using System.Net.Mail;
using FluentEmail.Core;

namespace Bff.Infrastructure.Extensions
{
    public class MailServices : IMailServices
    {
        private readonly SmtpClient _client;
        private readonly string _fromAddress;
        private readonly IConfiguration _configuration;
        private readonly IFluentEmail _fluentEmail;

        public MailServices(IConfiguration configuration, IFluentEmail fluentEmail, IFluentEmailFactory fluentEmailFactory)
        {
            _client = new SmtpClient(configuration["EmailSettings:Host"])
            {
                Port = int.Parse(configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(configuration["EmailSettings:Username"], configuration["EmailSettings:Password"]),
                EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"])
            };
            _fromAddress = configuration["EmailSettings:FromAddress"];
            _fluentEmail = fluentEmail;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string name, string otp)
        {

            var companyName = _configuration["EmailSettings:CompanyName"] ?? "Your Company";
            var fromAddress = _fromAddress; // already set from configuration

            var result = await _fluentEmail
                .SetFrom(fromAddress, companyName)
                .To(email, name)
                .Subject("Xác Minh Email")
                .UsingTemplateFromFile($"D:\\Fit\\ISD\\Active_BE\\Active_BE\\src\\Bff.Infrastructure\\Resources\\Templates\\Send_OTP.cshtml",
                    new { Name = name, OtpCode = otp })
                .SendAsync();

            if (!result.Successful)
            {
                throw new Exception($"Failed to send email: {result.ErrorMessages.FirstOrDefault()}");
            }
        }
    }
}
