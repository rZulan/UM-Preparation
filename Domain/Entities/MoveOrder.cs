using Domain.Entities.Junction;
using Domain.Entities.Masterlist;

namespace Domain.Entities;

public class MoveOrder : BaseEntity
{
    public bool IsTransacted { get; set; }

    public required int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public List<MoveOrderProducts> MoveOrderProducts { get; set; } = [];
}