using MediatR;

namespace LightCap.InvestmentApi.Application.Features.Queries;

public record GetInvestmentByIdQuery(Guid Id) : IRequest<InvestmentDto>;