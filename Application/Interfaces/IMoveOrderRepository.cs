using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
///     Provides data access operations for <see cref="MoveOrder" /> entities.
/// </summary>
public interface IMoveOrderRepository
{
    /// <summary>Returns a filtered and sorted list of all move orders.</summary>
    /// <param name="genericFiltersDTO">Search and pagination filters.</param>
    /// <param name="sort">Sort direction and field.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<List<MoveOrder>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
        CancellationToken cancellationToken);

    /// <summary>Returns the total count of move orders matching the given filters.</summary>
    /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<int> GetCountAsync(GenericFiltersDto genericFiltersDTO, CancellationToken cancellationToken);

    /// <summary>Returns a move order by its ID, or <see langword="null" /> if not found.</summary>
    /// <param name="id">The move order's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<MoveOrder?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Persists a new move order to the data store.</summary>
    /// <param name="moveOrder">The move order entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddAsync(MoveOrder moveOrder, CancellationToken cancellationToken);

    /// <summary>Saves changes to an existing move order.</summary>
    /// <param name="moveOrder">The move order entity with updated values.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateAsync(MoveOrder moveOrder, CancellationToken cancellationToken);
}