using Domain.Entities.Masterlist;

namespace Domain.Entities
{
    public class Dispatch : BaseEntity
    {
        public required string BatchNumber { get; set; }
        public required decimal QuantityOut { get; set; }
        public required decimal QuantityReturn { get; set; }
        public required DateTime HarvestDate { get; set; }

        public required int ProductId { get; set; }
        public required int FarmId { get; set; }
        public int? PreparedById { get; set; }
        public int? CheckedById { get; set; }
        public int? ReturnedById { get; set; }

        public Product Product { get; set; } = null!;
        public User? PreparedBy { get; set; }
        public User? CheckedBy { get; set; }
        public User? ReturnedBy { get; set; }
    }
}
