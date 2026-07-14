using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
///     Provides data access operations for <see cref="MiscellaneousReceipt" /> entities.
/// </summary>
public interface IMiscellaneousReceiptRepository
{
    /// <summary>Returns a filtered and sorted list of all miscellaneous receipts.</summary>
    /// <param name="genericFiltersDTO">Search and pagination filters.</param>
    /// <param name="sort">Sort direction and field.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<List<MiscellaneousReceipt>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
        CancellationToken cancellationToken);

    /// <summary>Returns the total count of miscellaneous receipts matching the given filters.</summary>
    /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<int> GetCountAsync(GenericFiltersDto genericFiltersDTO, CancellationToken cancellationToken);

    /// <summary>Returns a miscellaneous receipt by its ID, or <see langword="null" /> if not found.</summary>
    /// <param name="id">The miscellaneous receipt's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<MiscellaneousReceipt?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Persists a new miscellaneous receipt to the data store.</summary>
    /// <param name="miscellaneousReceipt">The miscellaneous receipt entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken);

    /// <summary>Saves changes to an existing miscellaneous receipt.</summary>
    /// <param name="miscellaneousReceipt">The miscellaneous receipt entity with updated values.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken);
}