namespace Application.Interfaces
{
    /// <summary>
    /// Provides password hashing and verification services.
    /// </summary>
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Hashes a plain-text password.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The hashed representation of the password.</returns>
        string Hash(string password);

        /// <summary>
        /// Verifies a plain-text password against a stored hash.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hash">The stored password hash to compare against.</param>
        /// <returns><see langword="true"/> if the password matches the hash; otherwise <see langword="false"/>.</returns>
        bool Verify(string password, string hash);
    }
}
