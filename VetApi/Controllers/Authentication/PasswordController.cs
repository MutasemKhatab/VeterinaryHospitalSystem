using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vet.Models;
using VetApi.Utils;

namespace VetApi.Controllers.Authentication;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PasswordController(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration
) : ControllerBase {
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto? changePasswordDto) {
        if (changePasswordDto == null) {
            return BadRequest("Invalid data.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) {
            return Unauthorized();
        }

        var vetOwner = await userManager.FindByIdAsync(userId);
        if (vetOwner == null) {
            return NotFound("User not found.");
        }

        var result = await userManager.ChangePasswordAsync(vetOwner, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded) {
            return BadRequest(result.Errors);
        }

        return Ok("Password changed successfully.");
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto? forgotPasswordDto) {
        if (forgotPasswordDto == null) {
            return BadRequest("Invalid data.");
        }

        var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null) {
            return NotFound("User not found.");
        }

        var resetCode = new Random().Next(100000, 999999).ToString(); // Generate a 6-digit code
        user.ResetCode = resetCode; // Assuming you have a ResetCode property in your ApplicationUser class
        await userManager.UpdateAsync(user);

        var smtpSettings = configuration.GetSection("Smtp").Get<SmtpSettings>();
        var mailMessage = new MailMessage {
            From = new MailAddress(smtpSettings.From),
            Subject = "Password Reset Code",
            Body = $"Your password reset code is: {resetCode}",
            IsBodyHtml = true
        };
        mailMessage.To.Add(user.Email);

        using (var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)) {
            smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
            smtpClient.EnableSsl = true;
            await smtpClient.SendMailAsync(mailMessage);
        }

        return Ok("Password reset code has been sent to your email.");
    }

    [AllowAnonymous]
    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDto? verifyResetCodeDto) {
        if (verifyResetCodeDto == null) {
            return BadRequest("Invalid data.");
        }

        var user = await userManager.FindByEmailAsync(verifyResetCodeDto.Email);
        if (user == null) {
            return NotFound("User not found.");
        }

        if (user.ResetCode != verifyResetCodeDto.ResetCode) {
            return BadRequest("Invalid reset code.");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, verifyResetCodeDto.NewPassword);
        if (!result.Succeeded) {
            return BadRequest(result.Errors);
        }

        user.ResetCode = null; // Clear the reset code after successful reset
        await userManager.UpdateAsync(user);

        return Ok("Password has been reset successfully.");
    }
}