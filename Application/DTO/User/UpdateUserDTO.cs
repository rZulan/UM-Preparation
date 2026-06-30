namespace Application.DTO.User
{
    public class UpdateUserDTO
    {
        public string? Username { get; set; }
        public string? UpdatePassword { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? IDPrefix { get; set; }
        public string? IDNumber { get; set; }
        public int? RoleId { get; set; }
    }
}
