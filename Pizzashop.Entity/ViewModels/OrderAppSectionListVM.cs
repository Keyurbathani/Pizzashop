namespace Pizzashop.Entity.ViewModels;

public class OrderAppSectionListVM
{
    public int SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? IsAvailableCount { get; set; }
    public int? IsAssignedCount { get; set; }
    public int? IsRunningCount { get; set; }

    public List<TableListNew> TableLists { get; set; } = new();
}

public class TableListNew
{
    public int? TableId { get; set; }
    public int? SectionId { get; set; }
    public string? TableName { get; set; }
    public int? Capacity { get; set; }
    public DateTime? Time { get; set; }
    public string? Status { get; set; } = "Available";
    public decimal? TotalAmount { get; set; }
    public int? OrderId {get; set;}
}
