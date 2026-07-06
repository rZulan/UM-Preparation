using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WarehouseReceivingRepository(AppDbContext context) : IWarehouseReceivingRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<WarehouseReceiving>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Include(x => x.MiscellaneousReceipt);

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(w =>
                    w.Id.ToString().Contains(searchTerm) ||
                    w.WarehouseId.ToString().Contains(searchTerm) ||
                    w.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    w.Quantity.ToString().Contains(searchTerm) ||
                    w.ProductId.ToString().Contains(searchTerm) ||
                    w.Product.ItemCode.ToLower().Contains(searchTerm) ||
                    w.Product.Description.ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(w => w.Id),
                    "id" when isDsc => query.OrderByDescending(w => w.Id),
                    "warehouseid" when isAsc => query.OrderBy(w => w.WarehouseId),
                    "warehouseid" when isDsc => query.OrderByDescending(w => w.WarehouseId),
                    "warehouse" when isAsc => query.OrderBy(w => w.Warehouse.Name),
                    "warehouse" when isDsc => query.OrderByDescending(w => w.Warehouse.Name),
                    "quantity" when isAsc => query.OrderBy(w => w.Quantity),
                    "quantity" when isDsc => query.OrderByDescending(w => w.Quantity),
                    "productid" when isAsc => query.OrderBy(w => w.ProductId),
                    "productid" when isDsc => query.OrderByDescending(w => w.ProductId),
                    "productcode" when isAsc => query.OrderBy(w => w.Product.ItemCode),
                    "productcode" when isDsc => query.OrderByDescending(w => w.Product.ItemCode),
                    "productdescription" when isAsc => query.OrderBy(w => w.Product.Description),
                    "productdescription" when isDsc => query.OrderByDescending(w => w.Product.Description),
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
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings;

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(w =>
                    w.Id.ToString().Contains(searchTerm) ||
                    w.WarehouseId.ToString().Contains(searchTerm) ||
                    w.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    w.Quantity.ToString().Contains(searchTerm) ||
                    w.ProductId.ToString().Contains(searchTerm) ||
                    w.Product.ItemCode.ToLower().Contains(searchTerm) ||
                    w.Product.Description.ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<WarehouseReceiving?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings
                .Where(w => w.Id == id)
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Include(x => x.MiscellaneousReceipt);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<WarehouseReceiving>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings
                .Where(w => w.ProductId == productId)
                .Include(x => x.Product)
                .Include(x => x.MiscellaneousReceipt);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(WarehouseReceiving warehouse, CancellationToken cancellationToken)
        {
            await _context.WarehouseReceivings.AddAsync(warehouse, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(WarehouseReceiving warehouse, CancellationToken cancellationToken)
        {
            _context.WarehouseReceivings.Update(warehouse);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
