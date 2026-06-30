using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to authenticate a user and issue a JWT access token and refresh token.</summary>
    /// <param name="LoginDTO">The login credentials (username and password).</param>
    /// <param name="RefreshToken">An existing refresh token to revoke on re-login, or <see langword="null"/> if none.</param>
    public record LoginUserCommand(LoginUserDTO LoginDTO, string? RefreshToken) : IRequest<Result<LoginResultDTO>>;
    public class LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService, IJwtService jwtService, IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginUserCommand, Result<LoginResultDTO>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        public async Task<Result<LoginResultDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.LoginDTO.Username, cancellationToken);

            if (existingUser == null)
            {
                return Result<LoginResultDTO>.Failure("Invalid username or password", HttpStatusCode.Unauthorized);
            }

            var isPasswordValid = _passwordHasherService.Verify(request.LoginDTO.Password, existingUser.PasswordHash);

            if (isPasswordValid == false)
            {
                return Result<LoginResultDTO>.Failure("Invalid username or password", HttpStatusCode.Unauthorized);
            }

            if (existingUser.IsActive == false)
            {
                return Result<LoginResultDTO>.Failure("Your account is deactivated, please contact an administrator.");
            }

            if (request.RefreshToken != null)
            {
                var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

                if (existingRefreshToken != null)
                {
                    await _refreshTokenRepository.RevokeAsync(existingRefreshToken, cancellationToken);
                }
            }

            var roles = existingUser.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role!.Name)
                .ToArray() ?? [];

            var permissions = existingUser.UserRoles?
                .Where(ur => ur.Role != null)
                .SelectMany(ur => ur.Role!.RolePermissions)
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.Name)
                .Distinct()
                .ToArray() ?? [];

            var token = _jwtService.GenerateToken(existingUser.Id, existingUser.Username, roles, permissions);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = existingUser.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            var loginResult = new LoginResultDTO
            {
                Id = existingUser.Id,
                Username = existingUser.Username,
                FirstName = existingUser.FirstName,
                MiddleName = existingUser.MiddleName,
                LastName = existingUser.LastName,
                Suffix = existingUser.Suffix,
                IDPrefix = existingUser.IDPrefix,
                IDNumber = existingUser.IDNumber,
                Role = existingUser.UserRoles?
                    .Select(ur => ur.Role!.Name)
                    .FirstOrDefault() ?? "N/A",
                Permissions = existingUser.UserRoles?
                    .SelectMany(ur => ur.Role!.RolePermissions)
                    .Select(rp => rp.Permission!.Name)
                    .Distinct()
                    .ToList() ?? [],
                AccessToken = token,
                RefreshToken = refreshTokenValue,
            };

            return Result<LoginResultDTO>.Success(loginResult, "User logged in successfully", HttpStatusCode.OK);
        }
    }
}
