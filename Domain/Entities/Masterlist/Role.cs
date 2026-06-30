using Domain.Entities.Junction;

namespace Domain.Entities.Masterlist
{
    public class Role : BaseEntity
    {
        public required string Name { get; set; }

        public List<UserRoles>? UserRoles { get; set; }
        public List<RolePermissions> RolePermissions { get; set; } = [];
    }
}
