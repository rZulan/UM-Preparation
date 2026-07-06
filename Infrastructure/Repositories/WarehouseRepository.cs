using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities.Masterlist;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WarehouseRepository(AppDbContext context) : IWarehouseRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Warehouse>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<Warehouse> query = _context.Warehouses;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(w => w.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(w =>
                    w.Id.ToString().Contains(searchTerm) ||
                    w.Name.ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(w => w.Id),
                    "id" when isDsc => query.OrderByDescending(w => w.Id),
                    "name" when isAsc => query.OrderBy(w => w.Name),
                    "name" when isDsc => query.OrderByDescending(w => w.Name),
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
            IQueryable<Warehouse> query = _context.Warehouses;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(w => w.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(w =>
                    w.Id.ToString().Contains(searchTerm) ||
                    w.Name.ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Warehouse> query = _context.Warehouses
                .Where(w => w.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            IQueryable<Warehouse> query = _context.Warehouses
                .Where(w => w.Name.ToLower() == name.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken)
        {
            await _context.Warehouses.AddAsync(warehouse, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken)
        {
            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AnyDuplicateAsync(int id, string name, CancellationToken cancellationToken)
        {
            return await _context.Warehouses.AnyAsync(w => w.Id != id && w.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
