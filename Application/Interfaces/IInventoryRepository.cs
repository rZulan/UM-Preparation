using Application.DTO.Inventory;

namespace Application.Interfaces
{
    public interface IInventoryRepository
    {
        Task<GetInventoryDto?> GetByWarehouseIdAsync(int warehouseId, CancellationToken cancellationToken);
    }
}
