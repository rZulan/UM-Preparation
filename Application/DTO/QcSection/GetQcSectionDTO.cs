namespace Application.DTO.QcSection
{
    public class GetQcSectionDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int Order { get; set; }
        public required int QcFormId { get; set; }
    }
}
