namespace Application.DTO.Dispatch
{
    public class AddDispatchDTO
    {
        public required string BatchNumber { get; set; }
        public required int ProductId { get; set; }
        public required int FarmId { get; set; }
        public required decimal QuantityOut { get; set; }
        public required decimal QuantityReturn { get; set; }
        public required DateTime HarvestDate { get; set; }
    }
}