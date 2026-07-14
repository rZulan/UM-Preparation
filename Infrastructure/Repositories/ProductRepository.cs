using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities.Masterlist;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        public async Task<List<Product>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> query = context.Products
                .Include(x => x.Uom);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.ItemCode.ToLower().Contains(searchTerm) ||
                    x.Description.ToLower().Contains(searchTerm) ||
                    x.Uom.ShortName.ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
                    "itemcode" when isAsc => query.OrderBy(x => x.ItemCode),
                    "itemcode" when isDsc => query.OrderByDescending(x => x.ItemCode),
                    _ => query
                };
            }

            if (genericFiltersDTO.UsePagination)
            {
                query = query.Skip((genericFiltersDTO.PageNumber - 1) * genericFiltersDTO.PageSize)
                    .Take(genericFiltersDTO.PageSize);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(GenericFiltersDto genericFiltersDTO, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = context.Products;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.ItemCode.ToLower().Contains(searchTerm) ||
                    x.Description.ToLower().Contains(searchTerm) ||
                    x.Uom.ShortName.ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = context.Products
                .Where(x => x.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Product>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = context.Products
                .Where(x => ids.Contains(x.Id));

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = context.Products
                .Where(x => x.ItemCode.ToLower() == itemCode.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            await context.Products.AddAsync(product, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AnyDuplicateAsync(int id, string itemCode, CancellationToken cancellationToken)
        {
            return await context.Products.AnyAsync(x => x.Id != id && x.ItemCode.ToLower() == itemCode.ToLower(),
                cancellationToken);
        }

        public async Task<bool> AllExistsAsync(List<int> productIds, CancellationToken cancellationToken)
        {
            return await context.Products.CountAsync(x => productIds.Contains(x.Id), cancellationToken) ==
                   productIds.Count;
        }
    }
}