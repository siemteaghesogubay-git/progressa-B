using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using progressa.Services.IServices;

namespace progressa.Services
{
    public class AzureEmailService(IOptions<EmailSettings> options) : IEmailService
    {
        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var s = options.Value;
            var client = new EmailClient(s.ConnectionString);

            await client.SendAsync(
                WaitUntil.Started,   // vänta inte på leveransstatus
                s.FromAddress,
                to,
                subject,
                htmlBody);
        }
    }
}