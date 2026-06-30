namespace Application.DTO.QcSection
{
    public class UpdateQcSectionDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }
        public int? QcFormId { get; set; }
    }
}
