using Application.DTO.Inventory;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InventoryRepository(AppDbContext context) : IInventoryRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<GetInventoryDto?> GetByWarehouseIdAsync(int warehouseId, CancellationToken cancellationToken)
        {
            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .Where(x => x.Id == warehouseId)
                .Select(x => new { x.Id, x.Name })
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse == null)
            {
                return null;
            }

            var products = await _context.Products
                .AsNoTracking()
                .Select(x => new InventoryProductDto
                {
                    ProductId = x.Id,
                    ItemCode = x.ItemCode,
                    Description = x.Description,
                    Soh =
                        (_context.WarehouseReceivings
                            .Where(receiving =>
                                receiving.WarehouseId == warehouseId &&
                                receiving.ProductId == x.Id)
                            .Select(receiving => (decimal?)receiving.Quantity)
                            .Sum() ?? 0m) -
                        (_context.MoveOrderProductWarehouseReceivings
                            .Where(allocation =>
                                allocation.WarehouseReceiving.WarehouseId == warehouseId &&
                                allocation.WarehouseReceiving.ProductId == x.Id &&
                                allocation.MoveOrderProduct.MoveOrder.IsTransacted)
                            .Select(allocation => (decimal?)allocation.Quantity)
                            .Sum() ?? 0m),
                    Reserve =
                        (_context.WarehouseReceivings
                            .Where(receiving =>
                                receiving.WarehouseId == warehouseId &&
                                receiving.ProductId == x.Id)
                            .Select(receiving => (decimal?)receiving.Quantity)
                            .Sum() ?? 0m) -
                        (_context.MoveOrderProductWarehouseReceivings
                            .Where(allocation =>
                                allocation.WarehouseReceiving.WarehouseId == warehouseId &&
                                allocation.WarehouseReceiving.ProductId == x.Id)
                            .Select(allocation => (decimal?)allocation.Quantity)
                            .Sum() ?? 0m)
                })
                .OrderBy(x => x.ItemCode)
                .ToListAsync(cancellationToken);

            return new GetInventoryDto
            {
                WarehouseId = warehouse.Id,
                Warehouse = warehouse.Name,
                Products = products
            };
        }
    }
}
