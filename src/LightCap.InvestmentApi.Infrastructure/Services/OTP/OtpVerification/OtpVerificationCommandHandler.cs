using BCrypt.Net;
using FluentResults;
using LightCap.InvestmentApi.Application.Common.Interfaces;
using LightCap.InvestmentApi.Domain.Entities;
using LightCap.InvestmentApi.Infrastructure.Services.OTP.OtpVerification;
using MediatR;

namespace LightCap.InvestmentApi.Application.Features.Auth.OtpVerification.Commands;

public record OtpVerificationCommand(string Email, string OtpCode) : IRequest<Result<OtpVerificationCommandOutput>>;

public class OtpVerificationCommandHandler(
    IRepository<Otp> otpRepository)
    : IRequestHandler<OtpVerificationCommand, Result<OtpVerificationCommandOutput>>
{
    public async Task<Result<OtpVerificationCommandOutput>> Handle(
        OtpVerificationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // GET OTP
            var otp = await otpRepository.GetSingleAsync(
                x => x.Email == request.Email &&
                     !x.IsUsed);

            // OTP NOT FOUND
            if (otp == null)
            {
                return Result.Fail<OtpVerificationCommandOutput>(
                    "OTP does not exist.");
            }

            // OTP EXPIRED
            if (otp.ExpiryTime < DateTime.UtcNow)
            {
                return Result.Fail<OtpVerificationCommandOutput>(
                    "OTP has expired.");
            }

            // VERIFY HASHED OTP
            var isValidOtp = BCrypt.Net.BCrypt.Verify(request.OtpCode, otp.Code);

            if (!isValidOtp)
            {
                otp.AttemptCount++;

                await otpRepository.UpdateAsync(otp);

                await otpRepository.SaveChanges(cancellationToken);

                return Result.Fail<OtpVerificationCommandOutput>(
                    "Invalid OTP.");
            }

            // MARK OTP AS USED
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