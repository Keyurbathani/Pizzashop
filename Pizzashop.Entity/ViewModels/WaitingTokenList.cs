using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class WaitingTokenList
{
    public int? Id {get; set;}
    public int? CustomerId {get; set;}

    [Required(ErrorMessage = "Name is required.")]
    [RegularExpression("^[a-zA-Z ]{1,}$", ErrorMessage = "Name should only contain alphabets!")]
    [StringLength(20, ErrorMessage = "Maximum 20 characters exceeded")]
    public string? Name {get; set;} = null!;

    [Required(ErrorMessage = "PhoneNumber is required.")]
    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number!")]
    public long? Phone {get; set;} = null!;

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please Enter Valid Email Address.")]
    [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
    public string? Email {get; set;}
    public string? SectionName {get; set;}

     [Required(ErrorMessage = "Section is Required")]
    public int? SectionId {get; set;} = null! ;
    public int? TableId {get; set;}

    [Required(ErrorMessage = "No Of Person is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "No Of Person should be more than Zero")]
    public int? NoOfPerson {get; set;}

    public DateTime CreatedAt {get; set;}
}
