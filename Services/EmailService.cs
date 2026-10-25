
using System.Net;
using System.Net.Mail;

namespace TaskFlow.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetLink)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(
            _configuration["EmailSettings:SmtpPort"] ?? "587");
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var appPassword = _configuration["EmailSettings:AppPassword"];

        if (string.IsNullOrWhiteSpace(smtpServer) ||
            string.IsNullOrWhiteSpace(senderEmail) ||
            string.IsNullOrWhiteSpace(appPassword))
        {
            throw new InvalidOperationException(
                "Email settings are missing. Check Render environment variables.");
        }

        using var message = new MailMessage();
        message.From = new MailAddress(senderEmail, "TaskFlow");
        message.To.Add(email);
        message.Subject = "Reset Your TaskFlow Password";
        message.IsBodyHtml = true;

        message.Body = $"""
            <h2>TaskFlow Password Reset</h2>
            <p>We received a request to reset your password.</p>
            <p>Click the link below to choose a new password:</p>
            <p><a href="{WebUtility.HtmlEncode(resetLink)}">Reset Password</a></p>
            <p>If you did not request this, you can ignore this email.</p>
            """;

        using var smtp = new SmtpClient(smtpServer, smtpPort);
        smtp.EnableSsl = true;
        smtp.Credentials = new NetworkCredential(
            senderEmail, appPassword);

        await smtp.SendMailAsync(message);
    }
}
