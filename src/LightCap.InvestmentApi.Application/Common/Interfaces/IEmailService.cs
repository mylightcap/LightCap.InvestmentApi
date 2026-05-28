namespace LightCap.InvestmentApi.Application.Common.Interfaces;

public interface IEmailService
    {
    Task SendEmailWithFallback(string to, string subject, string body, bool isHtml = true);
}

