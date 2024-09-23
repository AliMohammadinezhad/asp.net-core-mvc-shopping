using Microsoft.AspNetCore.Identity.UI.Services;

namespace ShopProject.Utility;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // send email logic
        return Task.CompletedTask;
    }
}