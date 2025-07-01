using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Entity.ViewModels;

public class ItemListVM
{
    
    public int Id { get; set; }

    public int categoryId {get; set;}

    public string Name { get; set; } = null!;

    public bool? Type { get; set; }

    public decimal? Rate { get; set; }

    public int? Quantity { get; set; }

    public string? Imageurl { get; set; }

    public IFormFile? ProfileImage { get; set; }

    public bool IsAvailable { get; set; }
    public bool? IsFavourite { get; set; }
    public decimal? TaxPercentage { get; set; }
}
