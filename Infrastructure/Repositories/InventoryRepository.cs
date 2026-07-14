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

            List<InventoryProductDto> products = await _context.Products
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

            return new GetInventoryDto { WarehouseId = warehouse.Id, Warehouse = warehouse.Name, Products = products };
        }

        public async Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken)
        {
            if (!await _context.Warehouses.AsNoTracking().AnyAsync(x => x.Id == warehouseId, cancellationToken))
            {
                return null;
            }

            return new GetInventoryReceivingDto
            {
                Products = await GetReceivingProducts(warehouseId)
                    .OrderBy(x => x.ItemCode)
                    .ToListAsync(cancellationToken)
            };
        }

        public async Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseAndProductIdAsync(
            int warehouseId,
            int productId,
            CancellationToken cancellationToken)
        {
            if (!await _context.Warehouses.AsNoTracking().AnyAsync(x => x.Id == warehouseId, cancellationToken))
            {
                return null;
            }

            GetInventoryReceivingProductDto? product = await GetReceivingProducts(warehouseId)
                .Where(x => x.ProductId == productId)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }

            return new GetInventoryReceivingDto { Products = [product] };
        }

        private IQueryable<GetInventoryReceivingProductDto> GetReceivingProducts(int warehouseId)
        {
            return _context.Products
                .AsNoTracking()
                .Select(product => new GetInventoryReceivingProductDto
                {
                    ProductId = product.Id,
                    ItemCode = product.ItemCode,
                    Description = product.Description,
                    WarehouseReceivings = _context.WarehouseReceivings
                        .AsNoTracking()
                        .Where(receiving =>
                            receiving.WarehouseId == warehouseId &&
                            receiving.ProductId == product.Id)
                        .OrderBy(receiving => receiving.Id)
                        .Select(receiving => new GetInventoryReceivingProductWarehouseReceivingDto
                        {
                            WarehouseReceivingId = receiving.Id,
                            TotalQuantity = receiving.Quantity,
                            UsedQuantity = _context.MoveOrderProductWarehouseReceivings
                                .Where(allocation =>
                                    allocation.ProductId == product.Id &&
                                    allocation.WarehouseReceivingId == receiving.Id)
                                .Select(allocation => (decimal?)allocation.Quantity)
                                .Sum() ?? 0m,
                            AvailableQuantity = receiving.Quantity -
                                                (_context.MoveOrderProductWarehouseReceivings
                                                    .Where(allocation =>
                                                        allocation.ProductId == product.Id &&
                                                        allocation.WarehouseReceivingId == receiving.Id)
                                                    .Select(allocation => (decimal?)allocation.Quantity)
                                                    .Sum() ?? 0m)
                        })
                        .ToList()
                });
        }
    }
}