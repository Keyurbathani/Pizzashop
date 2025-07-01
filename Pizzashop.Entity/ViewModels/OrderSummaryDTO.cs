namespace Pizzashop.Entity.ViewModels;

public class OrderSummaryDTO
{
    public decimal TotalSales {get; set;}
    public int TotalOrders {get; set;}
    public decimal AvgOrderValue {get; set;}
    public TimeSpan AvgWaitingTime {get; set;}
}