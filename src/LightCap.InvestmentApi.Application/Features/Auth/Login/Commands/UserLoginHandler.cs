using FluentResults;
using LightCap.InvestmentApi.Application.Common.Interfaces;
using LightCap.InvestmentApi.Domain.Entities;
using MediatR;

namespace LightCap.InvestmentApi.Application.Features.Auth.Login.Commands
{
    public record UserLoginCommand(LoginDto loginDto) : IRequest<Result<UserLoginResponse>>;
    public class UserLoginHandler(
    IJwtService jwtService,
    IRepository<User> userRepository,
    IRepository<UserLogin> loginRepository
) : IRequestHandler<UserLoginCommand, Result<UserLoginResponse>>
    {
        public async Task<Result<UserLoginResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var dto = request.loginDto;

            var user = await userRepository.GetSingleAsync(u => u.Email == dto.Email);

            if (user == null)
                return Result.Fail("Invalid email or password.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid)
                return Result.Fail("Invalid email or password.");

            var accessToken = await jwtService.GenerateAccessTokenAsync(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            var login = new UserLogin
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = user.Email,


                LoginTime = DateTime.UtcNow,
                IsActive = true,

                IpAddress = dto.IpAddress,
                DeviceId = dto.DeviceId,
                DeviceName = dto.DeviceName,


                //RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
               
            };

            await loginRepository.AddAsync(login, cancellationToken);
            await loginRepository.SaveChanges(cancellationToken);

            return Result.Ok(new UserLoginResponse
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = login.RefreshTokenExpiryTime ?? DateTime.UtcNow.AddDays(7)
            });
        }
    }
}
