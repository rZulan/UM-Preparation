using Domain.Entities.Masterlist;

namespace Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        public required string BatchNumber { get; set; }
        public required decimal Quantity { get; set; }
        public required DateTime HarvestDate { get; set; }

        public required int ProductId { get; set; }
        public required int FarmId { get; set; }
        public int? MiscellaneousReceiptId { get; set; }

        public Product Product { get; set; } = null!;
        public MiscellaneousReceipt? MiscellaneousReceipt { get; set; }
    }
}
