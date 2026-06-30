namespace Application.DTO.QcCategory
{
    public class GetQcCategoryDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
