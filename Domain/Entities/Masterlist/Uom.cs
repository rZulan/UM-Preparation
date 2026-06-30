namespace Domain.Entities.Masterlist
{
    public class Uom : BaseEntity
    {
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public required bool IsInteger { get; set; }
    }
}
