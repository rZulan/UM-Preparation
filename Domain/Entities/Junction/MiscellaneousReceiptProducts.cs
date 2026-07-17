using Domain.Entities.Masterlist;

namespace Domain.Entities.Junction;

public class MiscellaneousReceiptProducts
{
    public required decimal Quantity { get; set; }

    public required int MiscellaneousReceiptId { get; set; }
    public required int ProductId { get; set; }

    public MiscellaneousReceipt MiscellaneousReceipt { get; set; } = null!;
    public Product Product { get; set; } = null!;
}