namespace Application.DTO.Dispatch
{
    public class GetDispatchDTO
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string BatchNumber { get; set; } = null!;
        public string ItemCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Uom { get; set; } = null!;
        public decimal QuantityOut { get; set; }
        public decimal QuantityReturn { get; set; }
        public DateTime HarvestDate { get; set; }
    }
}
