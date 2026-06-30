namespace Application.DTO.QcSection
{
    public class AddQcSectionDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int Order { get; set; }
        public required int QcFormId { get; set; }
    }
}
