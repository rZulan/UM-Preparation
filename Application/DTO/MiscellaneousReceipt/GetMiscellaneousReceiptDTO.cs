namespace Application.DTO.MiscellaneousReceipt
{
    public class GetMiscellaneousReceiptDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required int WarehouseId { get; set; }
        public required string Warehouse { get; set; }
        public required int ProductId { get; set; }
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
        public required string Uom { get; set; }
        public required decimal Quantity { get; set; }
        public required string Reason { get; set; }
    }
}
