using BCrypt.Net;
using FluentResults;
using LightCap.InvestmentApi.Application.Common.Interfaces;
using LightCap.InvestmentApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LightCap.InvestmentApi.Infrastructure.Services.OTP.OtpVerification;

public record OtpVerificationCommand(string Email, string OtpCode)
    : IRequest<Result<OtpVerificationCommandOutput>>;

public class OtpVerificationCommandHandler(IRepository<Otp> otpRepository)
    : IRequestHandler<OtpVerificationCommand, Result<OtpVerificationCommandOutput>>
{
    public async Task<Result<OtpVerificationCommandOutput>> Handle(
        OtpVerificationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            //  GET LATEST OTP FOR EMAIL
            var otp = await otpRepository.GetQueryable()
                .Where(x => x.Email == request.Email && !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (otp == null)
            {
                return Result.Fail<OtpVerificationCommandOutput>(
                    "No OTP request found for this email.");
            }

            
            if (otp.ExpiryTime < DateTime.UtcNow)
            {
                return Result.Fail<OtpVerificationCommandOutput>(
                    "OTP has expired.");
            }

            
            var isValidOtp = BCrypt.Net.BCrypt.Verify(request.OtpCode, otp.Code);

            if (!isValidOtp)
            {
                otp.AttemptCount++;

                await otpRepository.UpdateAsync(otp);
                await otpRepository.SaveChanges(cancellationToken);

                return Result.Fail<OtpVerificationCommandOutput>(
                    "Invalid OTP.");
            }

            
            otp.IsUsed = true;

            await otpRepository.UpdateAsync(otp);
            await otpRepository.SaveChanges(cancellationToken);

            
            return Result.Ok(new OtpVerificationCommandOutput
            {
                IsVerified = true,
                Email = request.Email,
                Message = "OTP verified successfully."
            });
        }
        catch (Exception ex)
        {
            return Result.Fail<OtpVerificationCommandOutput>(
                $"OTP verification failed: {ex.Message}");
        }
    }
}