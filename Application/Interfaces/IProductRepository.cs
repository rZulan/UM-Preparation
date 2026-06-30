using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Domain.Entities.Masterlist;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="Product"/> entities.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>Returns a filtered and sorted list of all products.</summary>
        /// <param name="genericFiltersDTO">Search and pagination filters.</param>
        /// <param name="sort">Sort direction and field.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<List<Product>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken);

        /// <summary>Returns the total count of products matching the given filters.</summary>
        /// <param name="genericFiltersDTO">Filters to apply before counting.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken);

        /// <summary>Returns a product by its ID, or <see langword="null"/> if not found.</summary>
        /// <param name="id">The product's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>Returns a product matching the given item code, or <see langword="null"/> if not found.</summary>
        /// <param name="itemCode">The exact item code to search for.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<Product?> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken);

        /// <summary>Persists a new product to the data store.</summary>
        /// <param name="Product">The product entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(Product Product, CancellationToken cancellationToken);

        /// <summary>Saves changes to an existing product.</summary>
        /// <param name="Product">The product entity with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(Product Product, CancellationToken cancellationToken);

        /// <summary>Checks whether another product (excluding the given ID) already uses the specified item code.</summary>
        /// <param name="id">The ID of the product to exclude from the check.</param>
        /// <param name="itemCode">The item code to check for duplicates.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><see langword="true"/> if a duplicate exists; otherwise <see langword="false"/>.</returns>
        Task<bool> CheckDuplicateAsync(int id, string itemCode, CancellationToken cancellationToken);
    }
}
