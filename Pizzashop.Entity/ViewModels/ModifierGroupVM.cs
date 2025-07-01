using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class ModifierGroupVM
{
    public int Id {get; set;}

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CreatedBy {get; set;}

    public int UpdatedBy {get; set;}
}
