namespace Application.DTO.PendingAccount
{
    public class AddPendingAccountDTO
    {
        public int Id { get; set; }
        public required string id_prefix { get; set; }
        public required string id_no { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string first_name { get; set; }
        public required string middle_name { get; set; }
        public required string last_name { get; set; }
        public required string Suffix { get; set; }
    }
}
