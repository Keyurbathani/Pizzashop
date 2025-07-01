namespace Pizzashop.Entity.ViewModels;

public class CustomerHistoryVM
{
    public int Id { get; set; }

    public string? Name {get; set;}

    public long? MobileNumber { get; set; }

    public decimal? MaxOrder { get; set; }

    public decimal? AverageBill { get; set; }

    public DateTime? CommingSince { get; set; }

    public int? Visits { get; set; }

    public List<CustomerHistory> customerHistories {get; set;} = new ();

   

}

public class CustomerHistory {

     public DateOnly? OrderDate { get; set; }

    public string? OrderType { get; set; }
    public string? Payment { get; set; }
    public int? Items { get; set; }

    public decimal? Amount { get; set; }

}
