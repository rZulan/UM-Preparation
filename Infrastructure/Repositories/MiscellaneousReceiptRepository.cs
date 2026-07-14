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

        public async Task<List<MiscellaneousReceipt>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
            CancellationToken cancellationToken)
        {
            IQueryable<MiscellaneousReceipt> query = _context.MiscellaneousReceipts
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .ThenInclude(x => x.Uom);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.WarehouseId.ToString().Contains(searchTerm) ||
                    x.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    x.ProductId.ToString().Contains(searchTerm) ||
                    x.Product.ItemCode.ToLower().Contains(searchTerm) ||
                    x.Product.Description.ToLower().Contains(searchTerm) ||
                    x.Product.Uom.ShortName.ToLower().Contains(searchTerm) ||
                    x.Quantity.ToString().Contains(searchTerm) ||
                    x.Reason.ToLower().Contains(searchTerm));
            }

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
                    "itemcode" when isAsc => query.OrderBy(x => x.Product.ItemCode),
                    "itemcode" when isDsc => query.OrderByDescending(x => x.Product.ItemCode),
                    "description" when isAsc => query.OrderBy(x => x.Product.Description),
                    "description" when isDsc => query.OrderByDescending(x => x.Product.Description),
                    "uom" when isAsc => query.OrderBy(x => x.Product.Uom.ShortName),
                    "uom" when isDsc => query.OrderByDescending(x => x.Product.Uom.ShortName),
                    "quantity" when isAsc => query.OrderBy(x => x.Quantity),
                    "quantity" when isDsc => query.OrderByDescending(x => x.Quantity),
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
            IQueryable<MiscellaneousReceipt> query = _context.MiscellaneousReceipts;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<MiscellaneousReceipt?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.MiscellaneousReceipts
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .ThenInclude(x => x.Uom)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken)
        {
            await _context.MiscellaneousReceipts.AddAsync(miscellaneousReceipt, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken)
        {
            _context.MiscellaneousReceipts.Update(miscellaneousReceipt);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}