using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Web.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Web.Services;

public interface IMailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

public class SendGridMailService : IMailService
{
    private SendGridConfig _sendGridConfig { get; } 
    private readonly ILogger _logger;

    public SendGridMailService(IOptions<SendGridConfig> sendGridConfig,
        ILogger<SendGridMailService> logger)
    {
        _sendGridConfig = sendGridConfig.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(_sendGridConfig.SendGridApiKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(_sendGridConfig.SendGridApiKey, subject, message, toEmail);
    }
    
    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("owen.mcgregor@gmail.com", "Password Recovery"),
            ReplyTo = new EmailAddress("no-reply@pix.com"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode 
            ? $"Email to {toEmail} queued successfully!"
            : $"Failure Email to {toEmail}");
    }
}
