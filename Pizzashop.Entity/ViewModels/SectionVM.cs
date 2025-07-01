namespace Pizzashop.Entity.ViewModels;

public class SectionVM
{
    public int Id {get; set;}
    public string Name {get; set;} = null!;
    public string? Description {get; set;}

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
    public int? TokenCount { get; set; }

    // public int? is
}
