using Domain.Entities.Masterlist;

namespace Domain.Entities
{
    public class MiscellaneousReceipt : BaseEntity
    {
        public required decimal Quantity { get; set; }

        public required int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}
