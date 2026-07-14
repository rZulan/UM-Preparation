using Application.DTO.Inventory;

namespace Application.Interfaces;

public interface IInventoryRepository
{
    Task<GetInventoryDto?> GetByWarehouseIdAsync(int warehouseId, CancellationToken cancellationToken);

    Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseIdAsync(int warehouseId,
        CancellationToken cancellationToken);

    Task<GetInventoryReceivingDto?> GetReceivingsByWarehouseAndProductIdAsync(int warehouseId, int productId,
        CancellationToken cancellationToken);
}