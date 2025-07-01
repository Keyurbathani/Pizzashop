namespace Pizzashop.Entity.ViewModels;

public class OrderListVM
{
    public int Order {get; set;}

    public DateOnly Date {get; set;}

    public string? CustomerName {get; set;}

    public string? Status {get; set;}

    public string? PaymentMode {get; set;}

    public decimal? Rating {get; set;}

    public decimal? Totalamount {get; set;}
}
