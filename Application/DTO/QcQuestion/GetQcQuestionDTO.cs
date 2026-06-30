namespace Application.DTO.QcQuestion
{
    public class GetQcQuestionDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Question { get; set; }
        public required bool IsRequired { get; set; }
        public string? CorrectAnswer { get; set; }
        public required int Order { get; set; }
        public required int QcSectionId { get; set; }
        public required int QcAnswerTypeId { get; set; }
    }
}
