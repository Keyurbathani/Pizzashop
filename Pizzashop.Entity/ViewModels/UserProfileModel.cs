using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Entity.ViewModels;

public class UserProfileModel
{
    public int Id {get; set;}

    [Required(ErrorMessage = "FirstName is required.")]
    [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "First name should only contain alphabets!")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string? FisrtName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required.")]
    [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "Last name should only contain alphabets!")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string? LastName { get; set; } = null!;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string Username { get; set; }

    [Required(ErrorMessage = "PhoneNumber is required.")]
    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone number!")]
    public long Phone { get; set; }

    public string? Email { get; set; }

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

    public string? Role {get; set;} 

     public string? Imageurl { get; set; }

    public IFormFile? ProfileImage {get; set;}
}
