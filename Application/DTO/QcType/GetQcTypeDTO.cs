namespace Application.DTO.QcType
{
    public class GetQcTypeDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
