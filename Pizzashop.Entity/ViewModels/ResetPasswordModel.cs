using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class ResetPasswordModel
{
    [Required(ErrorMessage = "New Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character, and be at least 8 characters long.")]
    [StringLength(10, ErrorMessage = "Maximum 10 characters exceeded")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage ="Confirm New password is required")]
    [Compare("NewPassword", ErrorMessage = "New Password and confirm New Password do not match.")]
    public string ConfirmPassword {get ; set;} = null!;
    public Guid Token {get ; set;}
    public string Email {get ; set;}
}
