using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class Modifier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public int UnitId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<ModifierAndGroup> ModifierAndGroups { get; set; } = new List<ModifierAndGroup>();

    public virtual ICollection<OrderedItemModifierMapping> OrderedItemModifierMappings { get; set; } = new List<OrderedItemModifierMapping>();

    public virtual Unit Unit { get; set; } = null!;
}
