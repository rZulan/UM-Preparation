namespace Application.DTO.MiscellaneousReceipt
{
    public class AddMiscellaneousReceiptDTO
    {
        public required int WarehouseId { get; set; }
        public required int ProductId { get; set; }
        public required decimal Quantity { get; set; }
        public required string Reason { get; set; }
    }
}