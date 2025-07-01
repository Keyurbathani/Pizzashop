using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class UnitVM
{
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Shortname { get; set; } = null!;
}
