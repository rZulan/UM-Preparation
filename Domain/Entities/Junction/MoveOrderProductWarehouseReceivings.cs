namespace Domain.Entities.Junction;

public class MoveOrderProductWarehouseReceivings
{
    public required decimal Quantity { get; set; }

    public required int MoveOrderId { get; set; }
    public required int ProductId { get; set; }
    public required int WarehouseReceivingId { get; set; }

    public MoveOrderProducts MoveOrderProduct { get; set; } = null!;
    public WarehouseReceiving WarehouseReceiving { get; set; } = null!;
}