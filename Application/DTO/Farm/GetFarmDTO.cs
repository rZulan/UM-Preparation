namespace Application.DTO.Farm
{
    public class GetFarmDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
    }
}
