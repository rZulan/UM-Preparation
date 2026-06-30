namespace Application.DTO.Role
{
    public class UpdateRoleDTO
    {
        public string? Name { get; set; }
        public List<int>? Permissions { get; set; }
    }
}
