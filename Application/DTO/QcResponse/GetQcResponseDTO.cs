namespace Application.DTO.QcResponse
{
    public class GetDispatchQcResponseDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string FormName { get; set; }
        public int? DispatchId { get; set; }
        public required string BatchNumber { get; set; }
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
        public required string Uom { get; set; }
        public required decimal QuantityOut { get; set; }
        public required decimal QuantityReturn { get; set; }
        public required DateTime HarvestDate { get; set; }
        public List<GetQcSectionForResponseDTO> Sections { get; set; } = [];
    }

    public class GetQcSectionForResponseDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<GetQcQuestionForResponseDTO> Questions { get; set; } = [];
    }

    public class GetQcQuestionForResponseDTO
    {
        public required string Question { get; set; }
        public string? Answer { get; set; }
    }
}
