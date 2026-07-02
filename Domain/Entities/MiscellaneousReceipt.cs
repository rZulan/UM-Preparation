using Domain.Entities.Masterlist;

namespace Domain.Entities
{
    public class MiscellaneousReceipt : BaseEntity
    {
        public required decimal Quantity { get; set; }
        public required string Reason { get; set; }

        public required int WarehouseId { get; set; }
        public required int ProductId { get; set; }

        public Warehouse Warehouse { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
