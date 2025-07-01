namespace Pizzashop.Entity.ViewModels;

public class CustomerListVM
{
    public int Id { get; set; }
    public string? Name {get; set;}
    public string? Email {get; set;}
    public long? Phone {get; set;}
    public DateOnly Date {get; set;}
    public int? TotalOrder {get; set;}

}
