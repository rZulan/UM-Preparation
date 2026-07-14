namespace Application.DTO.MoveOrder;

public class GetMoveOrderDto
{
    public required int Id { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsTransacted { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int WarehouseId { get; set; }
    public required string Warehouse { get; set; }
    public List<GetMoveOrderProductDTO> MoveOrderProducts { get; set; } = [];
}

public class GetMoveOrderProductDTO
{
    public required int ProductId { get; set; }
    public required string ItemCode { get; set; }
    public required string Description { get; set; }
    public required decimal TotalQuantity { get; set; }
}