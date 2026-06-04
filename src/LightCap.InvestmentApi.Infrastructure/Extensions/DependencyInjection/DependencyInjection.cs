using Azure.Storage.Blobs;
using CustOps.Infrastructure.Persistence.Repositories;
using FluentResults;
using LightCap.InvestmentApi.Application.Common.Interfaces;
using LightCap.InvestmentApi.Application.Features.Auth.OtpReset.Commands;
using LightCap.InvestmentApi.Domain.Entities;
using LightCap.InvestmentApi.Infrastructure.Persistence.DbContexts;
using LightCap.InvestmentApi.Infrastructure.Services.EmailService;
using LightCap.InvestmentApi.Infrastructure.Services.JWT;
using LightCap.InvestmentApi.Infrastructure.Services.Logger;
using LightCap.InvestmentApi.Infrastructure.Services.Logger.Slack;
using LightCap.InvestmentApi.Infrastructure.Services.OTP.OtpVerification;
using LightCap.InvestmentApi.Infrastructure.Services.OTP.TokenGenerator;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightCap.InvestmentApi.Infrastructure.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        //var connStr = configuration.GetConnectionString("LightCap");
        //services.AddDbContextFactory<AppDbContext>(options =>
        //    options.UseSqlServer(connStr, sqlOpt =>
        //        sqlOpt.EnableRetryOnFailure())
        //);


        // Blob
        var blobConnection = configuration.GetConnectionString("AzureBlobStorageConnection");

        if (!string.IsNullOrWhiteSpace(blobConnection))
        {
            services.AddSingleton(new BlobServiceClient(blobConnection));
        }




        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOtpService, OtpService>();
        
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<ISlackLogger, SlackLogger>();

        services.AddTransient<IRequestHandler<OtpVerificationCommand, Result<OtpVerificationCommandOutput>>, OtpVerificationCommandHandler>();
        services.AddTransient<IRequestHandler<OtpResetCommand, Result<OtpVerificationCommandOutput>>, OtpResetCommandHandler>();

        return services;
    }
}