namespace Pizzashop.Entity.ViewModels;

public class OrderedItemStatusVM
{
   
    public int OrderNumber { get; set; }
    public List<Items> Items { get; set; } = new();

}

public class Items
{
    public int Id { get; set; }

    public string? Name { get; set; } = null!;

    public int? Quantity { get; set; }

    public List<Modifiers> Modifiers { get; set; } = new();
}

public class Modifiers
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;


}

