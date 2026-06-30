namespace Application.DTO.QcAnswer
{
    public class AddQcAnswerDTO
    {
        public string? Answer { get; set; }
        public required int QcResponseId { get; set; }
        public required int QcQuestionId { get; set; }
    }
}
