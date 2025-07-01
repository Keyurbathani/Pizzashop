using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class ModifierVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is Required!")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Rate is Required!")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is Required!")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is Required!")]
    [Display(Name = "Unit")]
    public int? UnitId { get; set; }

    public string? Description { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

}
