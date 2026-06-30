namespace Application.DTO.User
{
    public class UpdatePasswordDTO
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
