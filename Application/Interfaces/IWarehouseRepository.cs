using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities.Masterlist;

namespace Application.Interfaces;

/// <summary>
///     Provides data access operations for <see cref="Warehouse" /> entities.
/// </summary>
public interface IWarehouseRepository
{
    /// <summary>Returns a filtered and sorted list of all warehouses.</summary>
    /// <param name="genericFiltersDTO">Search and pagination filters.</param>
    /// <param name="sort">Sort direction and field.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<List<Warehouse>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
        CancellationToken cancellationToken);

    /// <summary>Returns the total count of warehouses matching the given filters.</summary>
    /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<int> GetCountAsync(GenericFiltersDto genericFiltersDTO, CancellationToken cancellationToken);

    /// <summary>Returns a warehouse by its ID, or <see langword="null" /> if not found.</summary>
    /// <param name="id">The warehouse's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Returns a warehouse matching the given name, or <see langword="null" /> if not found.</summary>
    /// <param name="name">The exact warehouse name to search for.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>Persists a new warehouse to the data store.</summary>
    /// <param name="Warehouse">The warehouse entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddAsync(Warehouse Warehouse, CancellationToken cancellationToken);

    /// <summary>Saves changes to an existing warehouse.</summary>
    /// <param name="Warehouse">The warehouse entity with updated values.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateAsync(Warehouse Warehouse, CancellationToken cancellationToken);

    /// <summary>Checks whether another warehouse (excluding the given ID) already uses the specified name.</summary>
    /// <param name="id">The ID of the warehouse to exclude from the check.</param>
    /// <param name="name">The name to check for duplicates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><see langword="true" /> if a duplicate exists; otherwise <see langword="false" />.</returns>
    Task<bool> AnyDuplicateAsync(int id, string name, CancellationToken cancellationToken);
}