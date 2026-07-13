using Domain.Entities.Masterlist;

namespace Domain.Entities.Junction
{
    public class UserRoles
    {
        public required int UserId { get; set; }
        public required int RoleId { get; set; }

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
