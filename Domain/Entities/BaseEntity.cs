namespace Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }

        public User? CreatedBy { get; set; }
        public User? UpdatedBy { get; set; }
    }
}
