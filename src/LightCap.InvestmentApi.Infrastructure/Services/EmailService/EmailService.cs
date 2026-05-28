using LightCap.InvestmentApi.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace LightCap.InvestmentApi.Infrastructure.Services.EmailService;

public class EmailService(IConfiguration configuration) : IEmailService
{
    

    public async Task SendEmailWithFallback(string to, string subject, string body, bool isHtml = true)
    {
        var smtpServers = new[] {
        new { Host = configuration["Email:Host"], Port = int.Parse(configuration["Email:Port"]!) }
        // Add backups if needed
        };

        
       

        foreach (var server in smtpServers)
        {
            try
            {
                using var client = new SmtpClient(server.Host, server.Port)
                {
                    Credentials = new NetworkCredential(
                        configuration["Email:EmailHost"],
                        configuration["Email:Password"]
                    ),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage(configuration["Email:EmailHost"]!, to, subject, body);
                mailMessage.IsBodyHtml = true;

                await client.SendMailAsync(mailMessage);
                //return new EmailResponse { Success = true, Message = "Email sent successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP error with server {server.Host}: {ex.Message}");
                // Optionally log the error
            }
        }

        // If no SMTP server succeeded
        throw new Exception("All SMTP servers failed");
    }
}

