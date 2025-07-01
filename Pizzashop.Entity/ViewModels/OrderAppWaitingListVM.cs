namespace Pizzashop.Entity.ViewModels;

public class OrderAppWaitingListVM
{

    public int SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? TokenCount { get; set; }
    public List<WaitingList> WaitingList { get; set; } = new();
}

public class WaitingList
{
    public int? TokenId { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public long? Phone { get; set; }
    public string? Email { get; set; }
    public int? SectionId { get; set; }
    public int? TableId { get; set; }
    public int? NoOfPerson { get; set; }
    public DateTime CreatedAt { get; set; }
}
