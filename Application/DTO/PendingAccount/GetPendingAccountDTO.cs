namespace Application.DTO.PendingAccount
{
    public class GetPendingAccountDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string EmployeePrefix { get; set; }
        public required string EmployeeId { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Suffix { get; set; }
    }
}
