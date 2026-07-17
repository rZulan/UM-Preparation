using Domain.Entities.Junction;
using Domain.Entities.Masterlist;

namespace Domain.Entities;

public class MiscellaneousReceipt : BaseEntity
{
    public required string Reason { get; set; }

    public required int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public List<MiscellaneousReceiptProducts> MiscellaneousReceiptProducts { get; set; } = [];
    public List<WarehouseReceiving> WarehouseReceivings { get; set; } = [];
}
