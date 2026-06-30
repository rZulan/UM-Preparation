namespace Application.DTO.HorticultureClass
{
    public class GetHorticultureClassDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
