using MediatR;

namespace LightCap.InvestmentApi.Application.Investments.Commands.CreateInvestment;

public record CreateInvestmentCommand(decimal Amount, string Asset) : IRequest<Guid>;