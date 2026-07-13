using Domain.Entities.Masterlist;

namespace Domain.Entities.Junction
{
    public class MoveOrderProducts
    {
        public required decimal TotalQuantity { get; set; }

        public required int MoveOrderId { get; set; }
        public required int ProductId { get; set; }

        public MoveOrder MoveOrder { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public List<MoveOrderProductWarehouseReceivings> MoveOrderProductWarehouseReceivings { get; set; } = [];
    }
}
