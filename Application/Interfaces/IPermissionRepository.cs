using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities.Masterlist;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="Permission"/> entities.
    /// </summary>
    public interface IPermissionRepository
    {
        /// <summary>Returns a filtered and sorted list of all permissions.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<Permission>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of permissions matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a permission by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The permission's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns a permission matching the given name, or <see langword="null"/> if not found.</summary>
        /// <param name="name">The exact permission name to search for.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>Persists a new permission to the data store.</summary>
        /// <param name="Permission">The permission entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(Permission Permission, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing permission.</summary>
        /// <param name="Permission">The permission entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(Permission Permission, CancellationToken cancellationToken);

        /// <summary>Checks whether another permission (excluding the given ID) already uses the specified name.</summary>
        /// <param name="id">The ID of the permission to exclude from the check.</param>
        /// <param name="name">The name to check for duplicates.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><see langword="true"/> if a duplicate exists; otherwise <see langword="false"/>.</returns>
        Task<bool> CheckDuplicateAsync(int id, string name, CancellationToken cancellationToken);
    }
}
