using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="WarehouseReceiving"/> entities.
    /// </summary>
    public interface IWarehouseReceivingRepository
    {
        /// <summary>Returns a filtered and sorted list of all warehouse receiving entries.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<WarehouseReceiving>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of warehouse receiving entries matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a warehouse receiving entry by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The warehouse entry's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<WarehouseReceiving?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns warehouse receiving entries by product ID.</summary>
        /// <param name="productId">The product ID to search for.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<WarehouseReceiving>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);

        /// <summary>Persists a new warehouse receiving entry to the data store.</summary>
        /// <param name="warehouseReceiving">The warehouse entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(WarehouseReceiving warehouseReceiving, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing warehouse receiving entry.</summary>
        /// <param name="warehouseReceiving">The warehouse entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(WarehouseReceiving warehouseReceiving, CancellationToken cancellationToken);
    }
}
