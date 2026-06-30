namespace Application.DTO.Permission
{
    public class GetPermissionDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
