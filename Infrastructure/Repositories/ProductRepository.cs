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
        private readonly AppDbContext _context = context;

        public async Task<List<Product>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _context.Products
                .Include(x => x.Uom);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(p => p.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(p =>
                    p.ItemCode.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(p => p.Id),
                    "id" when isDsc => query.OrderByDescending(p => p.Id),
                    "itemcode" when isAsc => query.OrderBy(p => p.ItemCode),
                    "itemcode" when isDsc => query.OrderByDescending(p => p.ItemCode),
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

        public async Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _context.Products;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(p => p.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(p =>
                    p.ItemCode.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _context.Products
                .Where(p => p.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Product?> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _context.Products
                .Where(p => p.ItemCode.ToLower() == itemCode.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AnyDuplicateAsync(int id, string itemCode, CancellationToken cancellationToken)
        {
            return await _context.Products.AnyAsync(p => p.Id != id && p.ItemCode.ToLower() == itemCode.ToLower(), cancellationToken);
        }
    }
}
