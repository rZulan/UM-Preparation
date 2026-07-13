namespace Application.DTO.Product
{
    public class AddProductDto
    {
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
        public required int UomId { get; set; }
    }
}
