namespace Application.DTO.WarehouseReceiving
{
    public class AddWarehouseReceivingDTO
    {
        public required decimal Quantity { get; set; }
        public required int ProductId { get; set; }
        public required int WarehouseId { get; set; }
        public int? MiscellaneousReceiptId { get; set; }
    }
}
