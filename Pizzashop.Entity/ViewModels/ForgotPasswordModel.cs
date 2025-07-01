using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class ForgotPasswordModel {

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;

   
}