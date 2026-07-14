namespace Application.DTO.MoveOrder;

public class AddMoveOrderDto
{
    public required int WarehouseId { get; set; }

    public List<AddMoveOrderProductsDTO> AddMoveOrderProducts { get; set; } = [];
}

public class AddMoveOrderProductsDTO
{
    public required decimal Quantity { get; set; }

    public required int ProductId { get; set; }
}