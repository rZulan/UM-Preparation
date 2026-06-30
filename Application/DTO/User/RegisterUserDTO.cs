namespace Application.DTO.User
{
    public class RegisterUserDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public string? Suffix { get; set; }
        public required string IDPrefix { get; set; }
        public required string IDNumber { get; set; }
        public required int RoleId { get; set; }
    }
}
