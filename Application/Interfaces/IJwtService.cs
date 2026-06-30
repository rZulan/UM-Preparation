namespace Application.Interfaces
{
    /// <summary>
    /// Provides JWT token generation services.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a signed JWT access token for the given user.
        /// </summary>
        /// <param name="userId">The user's unique identifier to embed in the token claims.</param>
        /// <param name="username">The username to embed in the token claims.</param>
        /// <param name="roles">The roles assigned to the user.</param>
        /// <param name="permissions">The permissions granted to the user.</param>
        /// <returns>A signed JWT access token string.</returns>
        string GenerateToken(int userId, string username, IEnumerable<string> roles, IEnumerable<string> permissions);

        /// <summary>
        /// Generates a cryptographically random refresh token string.
        /// </summary>
        /// <returns>A new opaque refresh token string.</returns>
        string GenerateRefreshToken();
    }
}
