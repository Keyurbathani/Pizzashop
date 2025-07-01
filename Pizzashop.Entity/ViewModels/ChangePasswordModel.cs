using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "CurrentPassword is required")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "NewPassword is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character, and be at least 8 characters long.")]
    [StringLength(10)]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage ="ConfirmNewPassword is required")]
    [Compare("NewPassword", ErrorMessage = "NewPassword and confirmNewPassword do not match.")]
    [StringLength(10)]
    public string ConfirmPassword {get ; set;}  = null!;

}
