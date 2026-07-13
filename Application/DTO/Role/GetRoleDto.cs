using Application.DTO.Permission;

namespace Application.DTO.Role
{
    public class GetRoleDto
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
        public required List<GetPermissionDto> Permissions { get; set; }
    }
}
