using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MiscellaneousReceiptRepository(AppDbContext context) : IMiscellaneousReceiptRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<MiscellaneousReceipt>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<MiscellaneousReceipt> query = _context.MiscellaneousReceipt
                .Include(d => d.Product)
                    .ThenInclude(x => x.Uom);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(d => d.IsActive == genericFiltersDTO.IsActive);
            }

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(d => d.Id),
                    "id" when isDsc => query.OrderByDescending(d => d.Id),
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
            IQueryable<MiscellaneousReceipt> query = _context.MiscellaneousReceipt;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(d => d.IsActive == genericFiltersDTO.IsActive);
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<MiscellaneousReceipt?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.MiscellaneousReceipt.Include(d => d.Product).FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task AddAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken)
        {
            await _context.MiscellaneousReceipt.AddAsync(miscellaneousReceipt, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken)
        {
            _context.MiscellaneousReceipt.Update(miscellaneousReceipt);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}