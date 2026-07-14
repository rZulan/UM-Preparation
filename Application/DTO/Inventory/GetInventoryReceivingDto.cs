namespace Application.DTO.Inventory;

public class GetInventoryReceivingDto
{
    public List<GetInventoryReceivingProductDto> Products { get; set; } = [];
}

public class GetInventoryReceivingProductDto
{
    public required int ProductId { get; set; }
    public required string ItemCode { get; set; }
    public required string Description { get; set; }
    public List<GetInventoryReceivingProductWarehouseReceivingDto> WarehouseReceivings { get; set; } = [];
}

public class GetInventoryReceivingProductWarehouseReceivingDto
{
    public required int WarehouseReceivingId { get; set; }
    public required decimal TotalQuantity { get; set; }
    public required decimal UsedQuantity { get; set; }
    public required decimal AvailableQuantity { get; set; }
}