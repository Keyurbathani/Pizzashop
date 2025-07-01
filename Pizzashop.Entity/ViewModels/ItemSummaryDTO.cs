namespace Pizzashop.Entity.ViewModels;

public class ItemSummaryDTO
{
    public int Id {get; set;}
    public string Name {get; set;} = null!;
    public string? Image {get; set;}
    public int OrderCount {get; set;}
}