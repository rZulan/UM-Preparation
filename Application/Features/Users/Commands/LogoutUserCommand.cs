using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to log out a user by revoking their active refresh token.</summary>
    /// <param name="RefreshToken">The refresh token string to revoke.</param>
    public record LogoutUserCommand(string RefreshToken) : IRequest<Result<object>>;
    public class LogoutUserCommandHandler(IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LogoutUserCommand, Result<object>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        public async Task<Result<object>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (existing == null)
            {
                return Result<object>.Success(null, "User logged out successfully (no active session found)", HttpStatusCode.Accepted);
            }

            await _refreshTokenRepository.RevokeAsync(existing, cancellationToken);

            return Result<object>.Success(null, "User logged out successfully", HttpStatusCode.OK);
        }
    }
}
