using LightCap.InvestmentApi.Application.Common.Interfaces;
using LightCap.InvestmentApi.Infrastructure.Services.EmailService;
using Microsoft.AspNetCore.Mvc;

namespace LightCap.InvestmentApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
      
        try
        {
            await _emailService.SendEmailWithFallback(request.To, request.Subject, request.Body
            );

            return Ok(new
            {
                success = true,
                message = "Email sent successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to send email",
                error = ex.Message
            });
        }
    }
}