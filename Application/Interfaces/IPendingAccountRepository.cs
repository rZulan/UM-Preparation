using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="PendingAccount"/> entities.
    /// </summary>
    public interface IPendingAccountRepository
    {
        /// <summary>Returns a filtered and sorted list of all pending accounts.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<PendingAccount>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of pending accounts matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a pending account by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The pending account's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<PendingAccount?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns a pending account matching the given employee prefix and ID number, or <see langword="null"/> if not found.</summary>
        /// <param name="employeePrefix">The prefix part of the employee ID (e.g. department code).</param>
        /// <param name="employeeId">The numeric part of the employee ID.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<PendingAccount?> GetByFullIdNoAsync(string employeePrefix, string employeeId, CancellationToken cancellationToken);

        /// <summary>Persists a new pending account to the data store.</summary>
        /// <param name="PendingAccount">The pending account entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(PendingAccount PendingAccount, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing pending account.</summary>
        /// <param name="PendingAccount">The pending account entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(PendingAccount PendingAccount, CancellationToken cancellationToken);
    }
}
