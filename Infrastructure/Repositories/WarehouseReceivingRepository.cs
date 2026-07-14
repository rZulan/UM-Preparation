using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Features.MoveOrders.Commands;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WarehouseReceivingRepository(AppDbContext context) : IWarehouseReceivingRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<WarehouseReceiving>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
            CancellationToken cancellationToken)
        {
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .ThenInclude(x => x.Uom)
                .Include(x => x.MiscellaneousReceipt);

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.WarehouseId.ToString().Contains(searchTerm) ||
                    x.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    x.Quantity.ToString().Contains(searchTerm) ||
                    x.ProductId.ToString().Contains(searchTerm) ||
                    x.Product.ItemCode.ToLower().Contains(searchTerm) ||
                    x.Product.Description.ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
                    "warehouseid" when isAsc => query.OrderBy(x => x.WarehouseId),
                    "warehouseid" when isDsc => query.OrderByDescending(x => x.WarehouseId),
                    "warehouse" when isAsc => query.OrderBy(x => x.Warehouse.Name),
                    "warehouse" when isDsc => query.OrderByDescending(x => x.Warehouse.Name),
                    "quantity" when isAsc => query.OrderBy(x => x.Quantity),
                    "quantity" when isDsc => query.OrderByDescending(x => x.Quantity),
                    "productid" when isAsc => query.OrderBy(x => x.ProductId),
                    "productid" when isDsc => query.OrderByDescending(x => x.ProductId),
                    "productcode" when isAsc => query.OrderBy(x => x.Product.ItemCode),
                    "productcode" when isDsc => query.OrderByDescending(x => x.Product.ItemCode),
                    "productdescription" when isAsc => query.OrderBy(x => x.Product.Description),
                    "productdescription" when isDsc => query.OrderByDescending(x => x.Product.Description),
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
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings;

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.WarehouseId.ToString().Contains(searchTerm) ||
                    x.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    x.Quantity.ToString().Contains(searchTerm) ||
                    x.ProductId.ToString().Contains(searchTerm) ||
                    x.Product.ItemCode.ToLower().Contains(searchTerm) ||
                    x.Product.Description.ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<WarehouseReceiving?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings
                .Where(x => x.Id == id)
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .ThenInclude(x => x.Uom)
                .Include(x => x.MiscellaneousReceipt);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<WarehouseReceiving>> GetByProductIdAsync(int productId,
            CancellationToken cancellationToken)
        {
            IQueryable<WarehouseReceiving> query = _context.WarehouseReceivings
                .Where(x => x.ProductId == productId)
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

        public async Task<bool> ProductHasAvailableReserve(int warehouseId, int productId, decimal quantity,
            CancellationToken cancellationToken)
        {
            decimal totalReceived = await _context.WarehouseReceivings
                .Where(x =>
                    x.IsActive &&
                    x.WarehouseId == warehouseId &&
                    x.ProductId == productId
                )
                .SumAsync(x => x.Quantity, cancellationToken);

            decimal totalMoved = await _context.MoveOrderProductWarehouseReceivings
                .Where(x =>
                    x.MoveOrderProduct.MoveOrder.IsActive &&
                    x.WarehouseReceiving.WarehouseId == warehouseId &&
                    x.WarehouseReceiving.ProductId == productId
                )
                .Select(x => (decimal?)x.Quantity)
                .SumAsync(cancellationToken) ?? 0m;

            decimal availableStock = totalReceived - totalMoved;

            return availableStock >= quantity;
        }

        public async Task<List<AvailableMoveOrderProductWarehouseReceivingsDto>> GetProductAffectedWarehouseReceivings(
            int warehouseId, int productId, decimal quantity, CancellationToken cancellationToken)
        {
            var receivingLots = await _context.WarehouseReceivings
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.WarehouseId == warehouseId &&
                    x.ProductId == productId
                )
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    AvailableQuantity = x.Quantity -
                                        (_context.MoveOrderProductWarehouseReceivings
                                            .Where(allocation =>
                                                allocation.WarehouseReceivingId == x.Id &&
                                                allocation.MoveOrderProduct.MoveOrder.IsActive)
                                            .Select(allocation => (decimal?)allocation.Quantity)
                                            .Sum() ?? 0m)
                })
                .ToListAsync(cancellationToken);

            List<AvailableMoveOrderProductWarehouseReceivingsDto> affectedWarehouseReceivings = new();
            decimal remainingQuantity = quantity;

            foreach (var receivingLot in receivingLots)
            {
                if (remainingQuantity <= 0)
                {
                    break;
                }

                decimal availableQuantity = Math.Max(0m, receivingLot.AvailableQuantity);
                if (availableQuantity == 0)
                {
                    continue;
                }

                decimal allocatedQuantity = Math.Min(remainingQuantity, availableQuantity);
                affectedWarehouseReceivings.Add(new AvailableMoveOrderProductWarehouseReceivingsDto
                {
                    ProductId = productId,
                    WarehouseReceivingId = receivingLot.Id,
                    AvailableQuantity = allocatedQuantity
                });

                remainingQuantity -= allocatedQuantity;
            }

            return affectedWarehouseReceivings;
        }
    }
}