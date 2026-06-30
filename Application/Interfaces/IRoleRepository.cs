using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities.Masterlist;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="Role"/> entities.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>Returns a filtered and sorted list of all roles.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<Role>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of roles matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a role by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The role's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns all roles whose IDs are in the given list.</summary>
        /// <param name="ids">The list of role IDs to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<Role>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken);

        /// <summary>Returns a role matching the given name, or <see langword="null"/> if not found.</summary>
        /// <param name="name">The exact role name to search for.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>Persists a new role to the data store.</summary>
        /// <param name="Role">The role entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(Role Role, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing role.</summary>
        /// <param name="Role">The role entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(Role Role, CancellationToken cancellationToken);

        /// <summary>Checks whether another role (excluding the given ID) already uses the specified name.</summary>
        /// <param name="id">The ID of the role to exclude from the check.</param>
        /// <param name="name">The name to check for duplicates.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><see langword="true"/> if a duplicate exists; otherwise <see langword="false"/>.</returns>
        Task<bool> CheckDuplicateAsync(int id, string name, CancellationToken cancellationToken);
    }
}
