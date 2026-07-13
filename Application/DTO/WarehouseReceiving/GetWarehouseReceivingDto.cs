namespace Application.DTO.WarehouseReceiving
{
    public class GetWarehouseReceivingDto
    {
        public required int Id { get; set; }
        public required int WarehouseId { get; set; }
        public required string Warehouse { get; set; }
        public required decimal Quantity { get; set; }
        public required int ProductId { get; set; }
        public required string ProductCode { get; set; }
        public required string ProductDescription { get; set; }
        public required bool IsInteger { get; set; }
    }
}
