namespace Pizzashop.Entity.ViewModels;

public class KotOrderVM
{
    public int OrderNumber { get; set; }

    public int CategoryId {get; set;}

    public DateTime Date { get; set; }

    public string? SectionName { get; set; }
    public List<string> TablesName { get; set; } = new();
    public string? Instruction {get; set;}
    public List<OrderItems> OrderItems { get; set; } = new();

}

public  class OrderItems
{
    public int Id { get; set; }
    public string? Name { get; set; } = null!;
    public int? Quantity {get; set;}
    public string? Instruction {get; set;}
    public List<OrderModifiers> OrderModifiers { get; set; } = new();
}

public class OrderModifiers
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

}
