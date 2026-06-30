namespace Application.DTO.QcQuestion
{
    public class UpdateQcQuestionDTO
    {
        public string? Question { get; set; }
        public bool? IsRequired { get; set; }
        public string? CorrectAnswer { get; set; }
        public int? Order { get; set; }
        public int? QcSectionId { get; set; }
        public int? QcAnswerTypeId { get; set; }
    }
}
