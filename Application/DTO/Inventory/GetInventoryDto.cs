namespace Application.DTO.Inventory;

public class GetInventoryDto
{
    public required int WarehouseId { get; set; }
    public required string Warehouse { get; set; }
    public List<InventoryProductDto> Products { get; set; } = [];
}

public class InventoryProductDto
{
    public required int ProductId { get; set; }
    public required string ItemCode { get; set; }
    public required string Description { get; set; }
    public required decimal Soh { get; set; }
    public required decimal Reserve { get; set; }
}