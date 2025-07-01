namespace Pizzashop.Entity.ViewModels;

public class ModifierGroupForItemVM
{
    public int Id { get; set; }
 
    public string? Name {get; set;}
 
    public int MinSelection { get; set; }
 
    public int MaxSelection { get; set; }
 
    public List<ModifierListVM> Modifiers {get; set;} = new();
}
