using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MoveOrderRepository(AppDbContext context) : IMoveOrderRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<MoveOrder>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<MoveOrder> query = _context.MoveOrders
                .Include(x => x.Warehouse)
                .Include(x => x.MoveOrderProducts)
                    .ThenInclude(x => x.Product);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
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
            IQueryable<MoveOrder> query = _context.MoveOrders;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<MoveOrder?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<MoveOrder> query = _context.MoveOrders
                .Where(x => x.Id == id)
                .Include(x => x.Warehouse)
                .Include(x => x.MoveOrderProducts)
                    .ThenInclude(x => x.Product);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(MoveOrder product, CancellationToken cancellationToken)
        {
            await _context.MoveOrders.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(MoveOrder product, CancellationToken cancellationToken)
        {
            _context.MoveOrders.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
