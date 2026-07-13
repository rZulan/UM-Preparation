namespace Application.DTO.Permission
{
    public class GetPermissionDto
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
