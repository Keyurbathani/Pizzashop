using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class TaxVM
{
    public int Id { get; set; }

    [StringLength(30)]
    [Required(ErrorMessage = "Name is Required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Select Tax Type First")]
    public bool? TaxType { get; set; } = null!;

    [Required(ErrorMessage = "TaxValue is required")]
    [Range(1, int.MaxValue, ErrorMessage = "TaxValue should be More than Zero")]
    public decimal? TaxValue { get; set; }

    public bool IsActive { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
}
