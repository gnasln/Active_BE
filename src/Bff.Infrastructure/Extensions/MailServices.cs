using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using Bff.Application.Contracts.Persistence;
using Bff.Application.Dtos.Mail;

namespace Bff.Infrastructure.Extensions
{
    public class MailServices : IMailServices
    {
        private readonly IConfiguration _configuration;

        public MailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendMailAsync(MailInfo mailInfo, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["MailSettings:FromName"], _configuration["MailSettings:FromAddress"]));
            message.To.Add(new MailboxAddress("", mailInfo.recipient));
            message.Subject = mailInfo.subject;
            message.Body = new TextPart(TextFormat.Html) { Text = mailInfo.body };
            try
            {
                using (var client = new SmtpClient())
                {
                    // Kết nối đến SMTP server
                    await client.ConnectAsync(_configuration["MailSettings:Host"], int.Parse(_configuration["MailSettings:Port"]), SecureSocketOptions.StartTls, cancellationToken);
                    // Xác thực
                    await client.AuthenticateAsync(_configuration["MailSettings:Username"], _configuration["MailSettings:Password"], cancellationToken);
                    // Gửi email
                    await client.SendAsync(message, cancellationToken);
                    // Ngắt kết nối
                    await client.DisconnectAsync(true, cancellationToken);
                }     
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false;
            }
        }
    }
}
