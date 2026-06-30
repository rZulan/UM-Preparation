using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="RefreshToken"/> entities.
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>Persists a new refresh token to the data store.</summary>
        /// <param name="refreshToken">The refresh token entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

        /// <summary>Returns the refresh token matching the given token string, or <see langword="null"/> if not found.</summary>
        /// <param name="token">The raw refresh token string to look up.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);

        /// <summary>Marks the given refresh token as revoked so it can no longer be used.</summary>
        /// <param name="refreshToken">The refresh token entity to revoke.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    }
}
