using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DispatchRepository(AppDbContext context) : IDispatchRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Dispatch>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<Dispatch> query = _context.Dispatches
                .Include(d => d.Product)
                    .ThenInclude(x => x.Uom);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(d => d.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(d => d.BatchNumber.Contains(genericFiltersDTO.SearchTerm));
            }

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(d => d.Id),
                    "id" when isDsc => query.OrderByDescending(d => d.Id),
                    "batchnumber" when isAsc => query.OrderBy(d => d.BatchNumber),
                    "batchnumber" when isDsc => query.OrderByDescending(d => d.BatchNumber),
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
            IQueryable<Dispatch> query = _context.Dispatches;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(d => d.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(d => d.BatchNumber.Contains(genericFiltersDTO.SearchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Dispatch?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Dispatches.Include(d => d.Product).FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsInSameBatchNumber(string batchNumber, CancellationToken cancellationToken)
        {
            return await _context.Dispatches.AnyAsync(q => q.BatchNumber == batchNumber, cancellationToken);
        }

        public async Task AddAsync(Dispatch dispatch, CancellationToken cancellationToken)
        {
            await _context.Dispatches.AddAsync(dispatch, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Dispatch dispatch, CancellationToken cancellationToken)
        {
            _context.Dispatches.Update(dispatch);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}