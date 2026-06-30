namespace Application.DTO.User
{
    public class GetUserDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Suffix { get; set; }
        public required string IDPrefix { get; set; }
        public required string IDNumber { get; set; }
        public required string Role { get; set; }
        public required List<string> Permissions { get; set; }
    }
}
