using Microsoft.AspNetCore.Mvc;
using OrganizandoBolso.API.Controllers.Base;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace OrganizandoBolso.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Email management")]
public class EmailController : BaseController
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public class SendEmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Send a simple email (Gmail SMTP)")]
    [SwaggerResponse(200, "Email sent successfully")]
    [SwaggerResponse(400, "Invalid data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<bool>>> Send([FromBody] SendEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.To))
        {
            return BadRequestResponse<bool>("Destination email is required");
        }

        var response = await _emailService.SendEmailAsync(request.To, request.Subject, request.Body, request.IsHtml);
        return HandleServiceResponse(response);
    }
}
