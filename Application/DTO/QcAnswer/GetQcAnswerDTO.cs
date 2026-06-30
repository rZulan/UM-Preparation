namespace Application.DTO.QcAnswer
{
    public class GetQcAnswerDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public string? Answer { get; set; }
        public required int QcResponseId { get; set; }
        public required int QcQuestionId { get; set; }
    }
}
