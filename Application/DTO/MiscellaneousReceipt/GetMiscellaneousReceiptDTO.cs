namespace Application.DTO.MiscellaneousReceipt
{
    public class GetMiscellaneousReceiptDTO
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string ItemCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Uom { get; set; } = null!;
        public decimal Quantity { get; set; }
    }
}
