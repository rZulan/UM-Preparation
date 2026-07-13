using Domain.Entities.Masterlist;

namespace Domain.Entities.Junction
{
    public class RolePermissions
    {
        public required int RoleId { get; set; }
        public required int PermissionId { get; set; }

        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
