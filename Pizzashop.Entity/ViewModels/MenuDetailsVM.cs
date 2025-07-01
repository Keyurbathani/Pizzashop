namespace Pizzashop.Entity.ViewModels;

public class MenuDetailsVM
{
    public string? ItemName {get; set;}
    public string? ModifierGroupName {get; set;}
    public int? ModifierGroupId {get; set;}
     public int? MaxSeclectionRequired {get; set;}
    public int? MinSeclectionRequired {get; set;}

    public List<ModifierList> ModifierList {get; set;} = new();

}

public class ModifierList
{
    public int? ModifierId {get; set;}
    public string? ModifierName {get; set;}
    public decimal? ModifierPrice {get; set;}

}
