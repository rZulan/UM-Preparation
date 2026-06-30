namespace Application.DTO.Product
{
    public class UpdateProductDTO
    {
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public int? UomId { get; set; }
    }
}
