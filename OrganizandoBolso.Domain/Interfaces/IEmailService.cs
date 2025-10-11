using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Interfaces;

public interface IEmailService
{
    Task<ServiceResponse<bool>> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
}
