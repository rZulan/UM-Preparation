namespace Application.DTO.Role
{
    public class UpdateRoleDto
    {
        public string? Name { get; set; }
        public List<int>? Permissions { get; set; }
    }
}
