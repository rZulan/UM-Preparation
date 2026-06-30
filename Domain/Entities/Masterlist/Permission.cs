using Domain.Entities.Junction;

namespace Domain.Entities.Masterlist
{
    public class Permission : BaseEntity
    {
        public required string Name { get; set; }

        public List<RolePermissions>? RolePermissions { get; set; }
    }
}
