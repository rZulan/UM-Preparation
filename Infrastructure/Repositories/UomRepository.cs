using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities.Masterlist;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UomRepository(AppDbContext context) : IUomRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Uom>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<Uom> query = _context.Uoms;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(u => u.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(u =>
                    u.Name.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(u => u.Id),
                    "id" when isDsc => query.OrderByDescending(u => u.Id),
                    "name" when isAsc => query.OrderBy(u => u.Name),
                    "name" when isDsc => query.OrderByDescending(u => u.Name),
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
            IQueryable<Uom> query = _context.Uoms;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(u => u.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(u =>
                    u.Name.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Uom?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Uom> query = _context.Uoms
                .Where(u => u.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Uom?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            IQueryable<Uom> query = _context.Uoms
                .Where(u => u.Name.ToLower() == name.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Uom uom, CancellationToken cancellationToken)
        {
            await _context.Uoms.AddAsync(uom, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Uom uom, CancellationToken cancellationToken)
        {
            _context.Uoms.Update(uom);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CheckDuplicateAsync(int id, string name, CancellationToken cancellationToken)
        {
            return await _context.Uoms.AnyAsync(u => u.Id != id && u.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
