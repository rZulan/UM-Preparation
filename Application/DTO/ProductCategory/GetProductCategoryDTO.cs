namespace Application.DTO.ProductCategory
{
    public class GetProductCategoryDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
