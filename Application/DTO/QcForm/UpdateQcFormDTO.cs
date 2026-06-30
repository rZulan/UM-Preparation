namespace Application.DTO.QcForm
{
    public class UpdateQcFormDTO
    {
        public List<UpdateQcFormSectionDTO> QcSections { get; set; } = [];
        public List<int> RemoveQcSections { get; set; } = [];
    }

    public class UpdateQcFormSectionDTO
    {
        public int? QcSectionId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public List<UpdateQcFormSectionQuestionDTO> QcQuestions { get; set; } = [];
        public List<int> RemoveQcQuestions { get; set; } = [];
    }

    public class UpdateQcFormSectionQuestionDTO
    {
        public int? QcQuestionId { get; set; }
        public string? Question { get; set; }
        public bool? IsRequired { get; set; }
        public string? CorrectAnswer { get; set; }
        public int? QcAnswerTypeId { get; set; }
    }
}
