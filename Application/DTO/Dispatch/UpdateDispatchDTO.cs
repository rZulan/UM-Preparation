namespace Application.DTO.Dispatch
{
    public class UpdateDispatchDTO
    {
        public string? BatchNumber { get; set; }
        public int? ProductId { get; set; }
        public int? FarmId { get; set; }
        public decimal? QuantityOut { get; set; }
        public decimal? QuantityReturn { get; set; }
        public DateTime? HarvestDate { get; set; }
    }
}