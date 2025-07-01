namespace Pizzashop.Entity.ViewModels;

public class TaxListVM
{

    public int Id {get; set;}
    public string Name { get; set; } = null!;

    public bool TaxType { get; set; }

    public decimal? TaxValue { get; set; }

    public bool IsActive { get; set; }

    public bool IsDefault { get; set; }

}
