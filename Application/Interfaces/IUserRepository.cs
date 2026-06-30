using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="User"/> entities.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Returns a filtered and sorted list of all users.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="userSort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<User>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort userSort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of users matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a user by their ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns a user matching the given employee prefix and ID number, or <see langword="null"/> if not found.</summary>
        /// <param name="employeePrefix">The prefix part of the employee ID (e.g. department code).</param>
        /// <param name="employeeId">The numeric part of the employee ID.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<User?> GetByFullIdNoAsync(string employeePrefix, string employeeId, CancellationToken cancellationToken);

        /// <summary>Returns a user matching the given username, or <see langword="null"/> if not found.</summary>
        /// <param name="username">The username to search for.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

        /// <summary>Persists a new user to the data store.</summary>
        /// <param name="user">The user entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(User user, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing user.</summary>
        /// <param name="user">The user entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(User user, CancellationToken cancellationToken);

        /// <summary>Checks whether another user (excluding the given ID) already uses the specified username.</summary>
        /// <param name="id">The ID of the user to exclude from the check.</param>
        /// <param name="username">The username to check for duplicates.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><see langword="true"/> if a duplicate exists; otherwise <see langword="false"/>.</returns>
        Task<bool> CheckDuplicateAsync(int id, string username, CancellationToken cancellationToken);
    }
}
