using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class MenuItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ProfileImage { get; set; }

    public int CategoryId { get; set; }

    public bool? ItemType { get; set; }

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public int? UnitId { get; set; }

    public bool? IsAvailable { get; set; }

    public decimal? TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public bool? IsDefaultTax { get; set; }

    public bool? IsFavourite { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual MenuCategory Category { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MappingMenuItemWithModifier> MappingMenuItemWithModifiers { get; set; } = new List<MappingMenuItemWithModifier>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    public virtual Unit? Unit { get; set; }
}
