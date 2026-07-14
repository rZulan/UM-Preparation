namespace Domain.Entities.Masterlist;

public class Product : BaseEntity
{
    public required string ItemCode { get; set; }
    public required string Description { get; set; }

    public required int UomId { get; set; }

    public Uom Uom { get; set; } = null!;
}