namespace Application.DTO.WarehouseReceiving
{
    public class UpdateWarehouseReceivingDTO
    {
        public decimal? Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? MiscellaneousReceiptId { get; set; }
    }
}
