using Application.DTO.Inventory;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;

namespace Application.Interfaces;

public interface IInventoryRepository
{
    Task<GetInventoryDto?> GetByWarehouseIdAsync(int warehouseId, GenericFiltersDto genericFiltersDto, Sort sort,
        CancellationToken cancellationToken);

    Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseIdAsync(int warehouseId,
        GenericFiltersDto genericFiltersDto,
        Sort sort,
        CancellationToken cancellationToken);

    Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseAndProductIdAsync(int warehouseId, int productId,
        GenericFiltersDto genericFiltersDto,
        Sort sort,
        CancellationToken cancellationToken);

    Task<int> GetProductCountAsync(GenericFiltersDto genericFiltersDto, int? productId,
        CancellationToken cancellationToken);
}
