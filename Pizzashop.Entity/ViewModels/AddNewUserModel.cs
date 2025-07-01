using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Entity.ViewModels;

public class AddNewUserModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "FirstName is required.")]
    [RegularExpression("^[a-zA-Z ]{1,}$", ErrorMessage = "First name should only contain alphabets!")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string? FisrtName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required.")]
    [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "Last name should only contain alphabets!")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string Username { get; set; }

    public bool? isActive { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public int? RoleId { get; set; } = null!;


    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please Enter Valid Email Address.")]
    [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
    public string? Email { get; set; } = null!;

    [Required(ErrorMessage = "PhoneNumber is required.")]
    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number!")]
    public long Phone { get; set; } 

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character, and be at least 8 characters long.")]
    [StringLength(10, ErrorMessage = "Maximum 10 characters exceeded")]
    public string? Password { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(100, ErrorMessage = "Maximum 50 characters exceeded")]
    public string? Address { get; set; } = null!;

    [Required(ErrorMessage = "ZipCode is required.")]
    [RegularExpression("[0-9]{6}", ErrorMessage = "Invalid Zip Code!")]
    public int? Zipcode { get; set; } = null!;

    [Required(ErrorMessage = "Country is required.")]
    public int? CountryId { get; set; } = null!;

    [Required(ErrorMessage = "State is required.")]
    public int? StateId { get; set; } = null!;

    [Required(ErrorMessage = "City is required.")]
    public int? CityId { get; set; } = null!;


    public string? Imageurl { get; set; }

    public IFormFile? ProfileImage { get; set; }

}
