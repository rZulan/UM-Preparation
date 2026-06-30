using Domain.Entities.Masterlist;

namespace Domain.Entities.Junction
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
