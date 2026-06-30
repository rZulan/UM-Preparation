namespace Domain.Entities
{
    public class PendingAccount : BaseEntity
    {
        public required string EmployeePrefix { get; set; }
        public required string EmployeeId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public string? Suffix { get; set; }
    }
}
