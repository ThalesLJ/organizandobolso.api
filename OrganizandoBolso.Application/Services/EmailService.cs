using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Domain.Models.Base;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace OrganizandoBolso.Application.Services;

public class EmailService : IEmailService
{
    private readonly ISettingRepository _settingRepository;
    private readonly ILogService _logService;

    public EmailService(ISettingRepository settingRepository, ILogService logService)
    {
        _settingRepository = settingRepository;
        _logService = logService;
    }

    public async Task<ServiceResponse<bool>> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var result = await SendEmailViaSmtpAsync(to, subject, body, isHtml);
            return result;
        }
        catch
        {
            await _logService.CreateAsync(new Log { Action = "EmailError", Message = "Error sending email" }, "system");
            return ServiceResponse<bool>.ErrorResponse("Error sending email", 500);
        }
    }

    private async Task<ServiceResponse<bool>> SendEmailViaSmtpAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var emailSetting = (await _settingRepository.GetByFilterAsync(s => s.Name == "EMAIL_ADDRESS")).FirstOrDefault()?.Value ?? string.Empty;
            var passwordSetting = (await _settingRepository.GetByFilterAsync(s => s.Name == "EMAIL_PASSWORD")).FirstOrDefault()?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(emailSetting) || string.IsNullOrEmpty(passwordSetting))
            {
                return ServiceResponse<bool>.ErrorResponse("Email settings not configured", 500);
            }

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(emailSetting, passwordSetting)
            };

            var message = new MailMessage
            {
                From = new MailAddress(emailSetting, "Organizando Tudo"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
            await _logService.CreateAsync(new Log { Action = "EmailSent", Message = "Email sent via SMTP" }, "system");
            return ServiceResponse<bool>.SuccessResponse(true, "Email sent successfully");
        }
        catch
        {
            await _logService.CreateAsync(new Log { Action = "EmailError", Message = "Error sending email via SMTP" }, "system");
            return ServiceResponse<bool>.ErrorResponse("Error sending email via SMTP", 500);
        }
    }
}
