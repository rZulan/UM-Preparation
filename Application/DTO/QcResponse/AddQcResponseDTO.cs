using Application.DTO.QcAnswer;

namespace Application.DTO.QcResponse
{
    public class AddQcResponseDTO
    {
        public required int QcFormId { get; set; }
        public int? DispatchId { get; set; }
        public List<AddQcAnswerForSectionDTO> QcAnswers { get; set; } = [];
    }
}
