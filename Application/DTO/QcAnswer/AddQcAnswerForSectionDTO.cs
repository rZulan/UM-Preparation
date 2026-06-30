namespace Application.DTO.QcAnswer
{
    public class AddQcAnswerForSectionDTO
    {
        public string? Answer { get; set; }
        public required int QcQuestionId { get; set; }
    }
}
