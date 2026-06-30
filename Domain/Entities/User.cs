using Domain.Entities.Junction;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public string? Suffix { get; set; }
        public required string IDPrefix { get; set; }
        public required string IDNumber { get; set; }

        public List<UserRoles> UserRoles { get; set; } = [];
    }
}
