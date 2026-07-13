namespace Application.DTO.MoveOrder
{
    public class ConsolidatedProductsDto
    {
        public required int Id { get; set; }
        public required decimal Quantity { get; set; }
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
    }
}
