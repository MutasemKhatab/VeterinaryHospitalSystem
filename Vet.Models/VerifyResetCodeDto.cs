namespace Vet.Models;

public class VerifyResetCodeDto {
    public string Email { get; set; }
    public string ResetCode { get; set; }
    public string NewPassword { get; set; }
}