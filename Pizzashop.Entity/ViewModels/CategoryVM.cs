using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class CategoryVM
{
    public int Id {get; set;}

    public string Name {get; set;} = null!;

    public string? Description {get; set;}
    
}
