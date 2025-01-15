using System.Web;
using Microsoft.AspNetCore.Identity;
using Web.Data;

namespace Web.Services;

public class EmailSender(IMailService mailService) : IEmailSender<ApplicationUser>
{
    public IMailService MailService { get; } = mailService;
    
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        await MailService.SendEmailAsync(email, "Email Confirmation", HttpUtility.HtmlDecode(confirmationLink));
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        await MailService.SendEmailAsync(email, "Password Reset Link", HttpUtility.HtmlDecode(resetLink));
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        await MailService.SendEmailAsync(email, "Password Reset Code", HttpUtility.HtmlDecode(resetCode));
    }
}