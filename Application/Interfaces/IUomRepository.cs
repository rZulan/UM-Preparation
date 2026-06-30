using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities.Masterlist;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="Uom"/> (Unit of Measure) entities.
    /// </summary>
    public interface IUomRepository
    {
        /// <summary>Returns a filtered and sorted list of all units of measure.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<Uom>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of units of measure matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a unit of measure by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The unit of measure's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Uom?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns a unit of measure matching the given name, or <see langword="null"/> if not found.</summary>
        /// <param name="name">The exact unit of measure name to search for.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Uom?> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>Persists a new unit of measure to the data store.</summary>
        /// <param name="Uom">The unit of measure entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(Uom Uom, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing unit of measure.</summary>
        /// <param name="Uom">The unit of measure entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(Uom Uom, CancellationToken cancellationToken);

        /// <summary>Checks whether another unit of measure (excluding the given ID) already uses the specified item code.</summary>
        /// <param name="id">The ID of the unit of measure to exclude from the check.</param>
        /// <param name="itemCode">The item code to check for duplicates.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><see langword="true"/> if a duplicate exists; otherwise <see langword="false"/>.</returns>
        Task<bool> CheckDuplicateAsync(int id, string itemCode, CancellationToken cancellationToken);
    }
}
