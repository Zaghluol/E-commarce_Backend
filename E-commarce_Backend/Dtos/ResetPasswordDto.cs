namespace E_commarce_Backend.Dtos
{
    public class ResetPasswordDto
    {
        public string Code { get; set; } 
        public string NewPassword { get; set; } 
        public string ConfirmNewPassword { get; set; }
    }
}
