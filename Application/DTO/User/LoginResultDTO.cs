namespace Application.DTO.User
{
    public class LoginResultDTO
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public string? Suffix { get; set; }
        public required string IDPrefix { get; set; }
        public required string IDNumber { get; set; }
        public required string Role { get; set; }
        public required List<string> Permissions { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
