using MediatR;
using FluentResults;
using LightCap.InvestmentApi.Application.Common.Interfaces;
using LightCap.InvestmentApi.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace LightCap.InvestmentApi.Application.Features.Auth.Registration.Commands
{
    public record UserRegistrationCommand(UserRegistrationDto UserRegistrationDto) : IRequest<Result<UserRegistrationResponse>>;
    public class UserRegistrationHandler(IRepository<User> repository, IEmailService emailService,
    IOtpService otpService,
    IRepository<Otp> otpRepository, IConfiguration config) : IRequestHandler<UserRegistrationCommand, Result<UserRegistrationResponse>>
    {
        public async Task<Result<UserRegistrationResponse>> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.UserRegistrationDto;

                var existingUser =  repository.Exists(x => x.Email == dto.Email);

                if (existingUser == true)
                {
                    return Result.Fail("User with this email already exists.");
                }

                if (dto.Password != dto.ConfirmPassword)
                {
                    return Result.Fail("Password and Confirm Password do not match.");
                }


                //OTP Generation and sending logic would go here (e.g., using an email service or SMS service)
                var otpLength = int.Parse(config["OTP:Length"]!);
                var generatedOtp = otpService.GenerateOtp();
                var hashedOtp = BCrypt.Net.BCrypt.HashPassword(generatedOtp);
                var expiryMinutes = int.Parse(config["OTP:ExpiryMinutes"]!);


                var emailSubject = "Email Verification OTP";

                var emailContent =
                    $"Your OTP for email verification is: {generatedOtp}. " +
                    $"It will expire in {expiryMinutes} minutes.";


                //await emailService.SendEmailWithFallback(
                //    dto.Email,
                //    emailSubject,
                //    emailContent);


                var otp = new Otp
                {
                    UserId = Guid.NewGuid(),
                    Email = dto.Email,
                    Code = hashedOtp,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    IsUsed = false,
                    AttemptCount = 0,
                    CreatedAt = DateTime.UtcNow
                };

                await otpRepository.AddAsync(otp, cancellationToken);

                await otpRepository.SaveChanges(cancellationToken);



                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    MiddleName = dto.MiddleName,
                    DateOfBirth = dto.DateOfBirth,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Gender = dto.Gender,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    AcceptTermsAndConditions = dto.AcceptTermsAndConditions,
                    AcceptPrivacyPolicy = dto.AcceptPrivacyPolicy,
                    DeviceId = dto.DeviceId,
                    DeviceName = dto.DeviceName,
                    DeviceType = dto.DeviceType,
                    IpAddress = dto.IpAddress,
                    Country = dto.Country,
                    State = dto.State,
                    City = dto.City
                };


                await repository.AddAsync(user, cancellationToken);
                await repository.SaveChanges(cancellationToken);

                return Result.Ok(new UserRegistrationResponse
                {
                    UserId = Guid.NewGuid(),
                    //FullName = request.UserRegistrationDto.FullName,
                    Email = request.UserRegistrationDto.Email,
                    PhoneNumber = request.UserRegistrationDto.PhoneNumber,
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    RequiresOtpVerification = true,
                    CreatedAt = DateTime.UtcNow,
                    Message = "User registered successfully. Please verify your email and phone number."
                });
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
