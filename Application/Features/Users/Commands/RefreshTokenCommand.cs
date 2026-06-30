using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to issue a new JWT access token using a valid refresh token.</summary>
    /// <param name="RefreshToken">The refresh token string to validate and exchange.</param>
    public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResultDTO>>;

    public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, Result<LoginResultDTO>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<Result<LoginResultDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (existing == null || existing.IsRevoked || existing.ExpiresAt < DateTime.UtcNow)
            {
                return Result<LoginResultDTO>.Failure("Invalid or expired refresh token.", HttpStatusCode.Unauthorized);
            }

            var user = await _userRepository.GetByIdAsync(existing.UserId, cancellationToken);

            if (user == null)
            {
                return Result<LoginResultDTO>.Failure("User not found.", HttpStatusCode.Unauthorized);
            }

            await _refreshTokenRepository.RevokeAsync(existing, cancellationToken);

            var roles = user.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role!.Name)
                .ToArray() ?? [];

            var permissions = user.UserRoles?
                .Where(ur => ur.Role != null)
                .SelectMany(ur => ur.Role!.RolePermissions)
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.Name)
                .Distinct()
                .ToArray() ?? [];

            var newAccessToken = _jwtService.GenerateToken(user.Id, user.Username, roles, permissions);
            var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            var loginResult = new LoginResultDTO
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Suffix = user.Suffix,
                IDPrefix = user.IDPrefix,
                IDNumber = user.IDNumber,
                Role = user.UserRoles?
                    .Select(ur => ur.Role!.Name)
                    .FirstOrDefault() ?? "N/A",
                Permissions = user.UserRoles?
                    .SelectMany(ur => ur.Role!.RolePermissions)
                    .Select(rp => rp.Permission!.Name)
                    .Distinct()
                    .ToList() ?? [],
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue
            };

            return Result<LoginResultDTO>.Success(loginResult, "Token refreshed successfully.", HttpStatusCode.OK);
        }
    }
}
