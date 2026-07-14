using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        }

        public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            refreshToken.IsRevoked = true;
            context.RefreshTokens.Update(refreshToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}