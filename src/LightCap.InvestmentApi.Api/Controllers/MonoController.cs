using LightCap.InvestmentApi.Application.Features.MonoHandler.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LightCap.InvestmentApi.Application.Features.MonoHandler.Command.MonoExchangeTokenHandler;

namespace LightCap.InvestmentApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonoController(ISender sender) : ControllerBase
    {
        public record ExchangeTokenRequest(string Code);

        [Authorize]
        [HttpPost("exchange-token")]
        public async Task<IActionResult> ExchangeToken(ExchangeTokenRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await sender.Send(new MonoExchangeTokenCommand(userId, request.Code));

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Errors.Select(e => e.Message));
        }
    }
}