using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TaskFlow.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public EmailService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetLink)
    {
        var apiKey = _configuration["RESEND_API_KEY"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "RESEND_API_KEY is missing from environment variables.");
        }

        var html = $"""
            <h2>TaskFlow Password Reset</h2>
            <p>We received a request to reset your password.</p>
            <p>Click the link below to choose a new password:</p>
            <p><a href="{WebUtility.HtmlEncode(resetLink)}">Reset Password</a></p>
            <p>If you did not request this, you can ignore this email.</p>
            """;

        var payload = new
        {
            from = "TaskFlow <onboarding@resend.dev>",
            to = new[] { email },
            subject = "Reset Your TaskFlow Password",
            html
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.resend.com/emails");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var httpClient = _httpClientFactory.CreateClient();

        using var response = await httpClient.SendAsync(request);
        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Resend email failed ({(int)response.StatusCode}): {responseBody}");
        }
    }
}
