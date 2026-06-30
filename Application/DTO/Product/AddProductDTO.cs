namespace Application.DTO.Product
{
    public class AddProductDTO
    {
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
        public required int ProductCategoryId { get; set; }
        public required int UomId { get; set; }
    }
}
