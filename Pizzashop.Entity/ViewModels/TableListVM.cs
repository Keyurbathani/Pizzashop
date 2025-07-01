namespace Pizzashop.Entity.ViewModels;

public class TableListVM
{
     public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int SectionId { get; set; }

    public int? Capacity { get; set; }

    public bool? IsAvailable { get; set; }
}
