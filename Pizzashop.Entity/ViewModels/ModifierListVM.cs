namespace Pizzashop.Entity.ViewModels;

public class ModifierListVM
{
    public int Id {get; set;}

    public string Name {get; set;} = null!;

    public string Unit {get; set;} = null!;

    public decimal Rate {get; set;}

    public int Quantity {get; set;}
}
