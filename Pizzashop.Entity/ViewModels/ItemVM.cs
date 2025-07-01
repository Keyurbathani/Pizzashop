using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Entity.ViewModels;

public class ItemVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category is Required")]
    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Name is Required")]
    public string Name { get; set; } = null!;


    [Display(Name = "Type")]
    [Required(ErrorMessage = "Type is Required")]
    public bool? Type { get; set; } = null!;

    [Required(ErrorMessage = "Rate is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "Rate should be More than Zero")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity should be More than Zero")]
    public int? Quantity { get; set; }

    [Display(Name = "Unit")]
    [Required(ErrorMessage = "Unit is Required")]
    public int? UnitId { get; set; }

    [StringLength(50)]
    public string? Shortcode { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    [Display(Name = "Default Tax")]
    [Required(ErrorMessage = "Default Tax is Required")]
    public bool IsDefaultTax { get; set; } = true;

    [Required(ErrorMessage = "Tax Percentage is Required")]
    [Range(0, int.MaxValue, ErrorMessage = "TaxPercentage should not be less than Zero")]
    public decimal? TaxPercentage { get; set; }

    public bool? IsFavourite { get; set; } 

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;

    public string? Imageurl { get; set; }

    public IFormFile? ProfileImage {get; set;} 

    public DateTime? ModifiedAt { get; set; }

    public List<ModifierGrouopForItem> SelectedModifierGroups {get; set;} = new();

}


public class ModifierGrouopForItem{
    public int Id {get; set;}

    public int MinSelection {get; set;}

    public int MaxSelection {get; set;}
}
