using Application.DTO.Inventory;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities.Masterlist;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InventoryRepository(AppDbContext context) : IInventoryRepository
    {
        public async Task<GetInventoryDto?> GetByWarehouseIdAsync(int warehouseId,
            GenericFiltersDto genericFiltersDto, Sort sort, CancellationToken cancellationToken)
        {
            var warehouse = await context.Warehouses
                .AsNoTracking()
                .Where(x => x.Id == warehouseId)
                .Select(x => new { x.Id, x.Name })
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse == null)
            {
                return null;
            }

            IQueryable<Product> query = context.Products.AsNoTracking();

            if (genericFiltersDto.IsActive.HasValue)
            {
                query = query.Where(product => product.IsActive == genericFiltersDto.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(genericFiltersDto.SearchTerm))
            {
                string searchTerm = genericFiltersDto.SearchTerm.ToLower();
                query = query.Where(product =>
                    product.Id.ToString().Contains(searchTerm) ||
                    product.ItemCode.ToLower().Contains(searchTerm) ||
                    product.Description.ToLower().Contains(searchTerm));
            }

            IQueryable<InventoryProductDto> productsQuery = query
                .Select(x => new InventoryProductDto
                {
                    ProductId = x.Id,
                    ItemCode = x.ItemCode,
                    Description = x.Description,
                    Soh =
                        (context.WarehouseReceivings
                            .Where(receiving =>
                                receiving.WarehouseId == warehouseId &&
                                receiving.ProductId == x.Id)
                            .Select(receiving => (decimal?)receiving.Quantity)
                            .Sum() ?? 0m) -
                        (context.MoveOrderProductWarehouseReceivings
                            .Where(allocation =>
                                allocation.WarehouseReceiving.WarehouseId == warehouseId &&
                                allocation.WarehouseReceiving.ProductId == x.Id &&
                                allocation.MoveOrderProduct.MoveOrder.IsTransacted)
                            .Select(allocation => (decimal?)allocation.Quantity)
                            .Sum() ?? 0m),
                    Reserve =
                        (context.WarehouseReceivings
                            .Where(receiving =>
                                receiving.WarehouseId == warehouseId &&
                                receiving.ProductId == x.Id)
                            .Select(receiving => (decimal?)receiving.Quantity)
                            .Sum() ?? 0m) -
                        (context.MoveOrderProductWarehouseReceivings
                            .Where(allocation =>
                                allocation.WarehouseReceiving.WarehouseId == warehouseId &&
                                allocation.WarehouseReceiving.ProductId == x.Id)
                            .Select(allocation => (decimal?)allocation.Quantity)
                            .Sum() ?? 0m)
                });

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                productsQuery = sort.SortBy.ToLower() switch
                {
                    "productid" when isAsc => productsQuery.OrderBy(product => product.ProductId),
                    "productid" when isDsc => productsQuery.OrderByDescending(product => product.ProductId),
                    "itemcode" when isAsc => productsQuery.OrderBy(product => product.ItemCode),
                    "itemcode" when isDsc => productsQuery.OrderByDescending(product => product.ItemCode),
                    "description" when isAsc => productsQuery.OrderBy(product => product.Description),
                    "description" when isDsc => productsQuery.OrderByDescending(product => product.Description),
                    "soh" when isAsc => productsQuery.OrderBy(product => product.Soh),
                    "soh" when isDsc => productsQuery.OrderByDescending(product => product.Soh),
                    "reserve" when isAsc => productsQuery.OrderBy(product => product.Reserve),
                    "reserve" when isDsc => productsQuery.OrderByDescending(product => product.Reserve),
                    _ => productsQuery.OrderBy(product => product.ItemCode)
                };
            }
            else
            {
                productsQuery = productsQuery.OrderBy(product => product.ItemCode);
            }

            if (genericFiltersDto.UsePagination)
                productsQuery = productsQuery
                    .Skip((genericFiltersDto.PageNumber - 1) * genericFiltersDto.PageSize)
                    .Take(genericFiltersDto.PageSize);

            List<InventoryProductDto> products = await productsQuery.ToListAsync(cancellationToken);

            return new GetInventoryDto { WarehouseId = warehouse.Id, Warehouse = warehouse.Name, Products = products };
        }

        public async Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseIdAsync(
            int warehouseId,
            GenericFiltersDto genericFiltersDto,
            Sort sort,
            CancellationToken cancellationToken)
        {
            if (!await context.Warehouses.AsNoTracking().AnyAsync(x => x.Id == warehouseId, cancellationToken))
            {
                return null;
            }

            IQueryable<GetInventoryReceivingProductDto> query = GetReceivingProducts(warehouseId, genericFiltersDto);

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "productid" when isAsc => query.OrderBy(product => product.ProductId),
                    "productid" when isDsc => query.OrderByDescending(product => product.ProductId),
                    "itemcode" when isAsc => query.OrderBy(product => product.ItemCode),
                    "itemcode" when isDsc => query.OrderByDescending(product => product.ItemCode),
                    "description" when isAsc => query.OrderBy(product => product.Description),
                    "description" when isDsc => query.OrderByDescending(product => product.Description),
                    _ => query.OrderBy(product => product.ItemCode)
                };
            }
            else
            {
                query = query.OrderBy(product => product.ItemCode);
            }

            if (genericFiltersDto.UsePagination)
                query = query
                    .Skip((genericFiltersDto.PageNumber - 1) * genericFiltersDto.PageSize)
                    .Take(genericFiltersDto.PageSize);

            return new GetInventoryReceivingDto
            {
                Products = await query.ToListAsync(cancellationToken)
            };
        }

        public async Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseAndProductIdAsync(
            int warehouseId,
            int productId,
            GenericFiltersDto genericFiltersDto,
            Sort sort,
            CancellationToken cancellationToken)
        {
            if (!await context.Warehouses.AsNoTracking().AnyAsync(x => x.Id == warehouseId, cancellationToken))
            {
                return null;
            }

            IQueryable<GetInventoryReceivingProductDto> query = GetReceivingProducts(warehouseId, genericFiltersDto)
                .Where(x => x.ProductId == productId);

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "productid" when isAsc => query.OrderBy(product => product.ProductId),
                    "productid" when isDsc => query.OrderByDescending(product => product.ProductId),
                    "itemcode" when isAsc => query.OrderBy(product => product.ItemCode),
                    "itemcode" when isDsc => query.OrderByDescending(product => product.ItemCode),
                    "description" when isAsc => query.OrderBy(product => product.Description),
                    "description" when isDsc => query.OrderByDescending(product => product.Description),
                    _ => query.OrderBy(product => product.ItemCode)
                };
            }
            else
            {
                query = query.OrderBy(product => product.ItemCode);
            }

            if (genericFiltersDto.UsePagination)
                query = query
                    .Skip((genericFiltersDto.PageNumber - 1) * genericFiltersDto.PageSize)
                    .Take(genericFiltersDto.PageSize);

            GetInventoryReceivingProductDto? product = await query.FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }

            return new GetInventoryReceivingDto { Products = [product] };
        }

        public async Task<int> GetProductCountAsync(GenericFiltersDto genericFiltersDto, int? productId,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> query = context.Products.AsNoTracking();

            if (genericFiltersDto.IsActive.HasValue)
            {
                query = query.Where(product => product.IsActive == genericFiltersDto.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(genericFiltersDto.SearchTerm))
            {
                string searchTerm = genericFiltersDto.SearchTerm.ToLower();
                query = query.Where(product =>
                    product.Id.ToString().Contains(searchTerm) ||
                    product.ItemCode.ToLower().Contains(searchTerm) ||
                    product.Description.ToLower().Contains(searchTerm));
            }

            if (productId.HasValue)
            {
                query = query.Where(product => product.Id == productId.Value);
            }

            return await query.CountAsync(cancellationToken);
        }

        private IQueryable<GetInventoryReceivingProductDto> GetReceivingProducts(int warehouseId,
            GenericFiltersDto genericFiltersDto)
        {
            IQueryable<Product> query = context.Products.AsNoTracking();

            if (genericFiltersDto.IsActive.HasValue)
            {
                query = query.Where(product => product.IsActive == genericFiltersDto.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(genericFiltersDto.SearchTerm))
            {
                string searchTerm = genericFiltersDto.SearchTerm.ToLower();
                query = query.Where(product =>
                    product.Id.ToString().Contains(searchTerm) ||
                    product.ItemCode.ToLower().Contains(searchTerm) ||
                    product.Description.ToLower().Contains(searchTerm));
            }

            return query
                .Select(product => new GetInventoryReceivingProductDto
                {
                    ProductId = product.Id,
                    ItemCode = product.ItemCode,
                    Description = product.Description,
                    WarehouseReceivings = context.WarehouseReceivings
                        .AsNoTracking()
                        .Where(receiving =>
                            receiving.WarehouseId == warehouseId &&
                            receiving.ProductId == product.Id)
                        .OrderBy(receiving => receiving.Id)
                        .Select(receiving => new GetInventoryReceivingProductWarehouseReceivingDto
                        {
                            WarehouseReceivingId = receiving.Id,
                            TotalQuantity = receiving.Quantity,
                            UsedQuantity = context.MoveOrderProductWarehouseReceivings
                                .Where(allocation =>
                                    allocation.ProductId == product.Id &&
                                    allocation.WarehouseReceivingId == receiving.Id)
                                .Select(allocation => (decimal?)allocation.Quantity)
                                .Sum() ?? 0m,
                            AvailableQuantity = receiving.Quantity -
                                                (context.MoveOrderProductWarehouseReceivings
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
