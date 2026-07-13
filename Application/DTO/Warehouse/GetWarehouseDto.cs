namespace Application.DTO.Warehouse
{
    public class GetWarehouseDto
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
