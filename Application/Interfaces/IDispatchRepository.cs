using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="Dispatch"/> entities.
    /// </summary>
    public interface IDispatchRepository
    {
        /// <summary>Returns a filtered and sorted list of all dispatches.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<Dispatch>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of dispatches matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a dispatch by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The dispatch's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Dispatch?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Checks whether a dispatch with the given batch number already exists.</summary>
        /// <param name="batchNumber">The batch number to check.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><see langword="true"/> if a dispatch with the same batch number exists; otherwise <see langword="false"/>.</returns>
        Task<bool> ExistsInSameBatchNumber(string batchNumber, CancellationToken cancellationToken);

        /// <summary>Persists a new dispatch to the data store.</summary>
        /// <param name="dispatch">The dispatch entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(Dispatch dispatch, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing dispatch.</summary>
        /// <param name="dispatch">The dispatch entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(Dispatch dispatch, CancellationToken cancellationToken);
    }
}