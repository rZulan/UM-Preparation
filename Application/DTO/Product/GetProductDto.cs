namespace Application.DTO.Product
{
    public class GetProductDto
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
        public required string Uom { get; set; }
    }
}
