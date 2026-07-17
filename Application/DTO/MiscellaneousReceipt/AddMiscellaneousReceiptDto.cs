namespace Application.DTO.MiscellaneousReceipt;

public class AddMiscellaneousReceiptDto
{
    public required int WarehouseId { get; set; }
    public required string Reason { get; set; }
    public List<AddMiscellaneousReceiptProductDto> AddMiscellaneousReceiptProducts { get; set; } = [];
}

public class AddMiscellaneousReceiptProductDto
{
    public required decimal Quantity { get; set; }

    public required int ProductId { get; set; }
}
