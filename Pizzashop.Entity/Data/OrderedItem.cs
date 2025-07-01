using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class OrderedItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int MenuItemId { get; set; }

    public int Quantity { get; set; }

    public decimal? ItemTotal { get; set; }

    public decimal? ItemRate { get; set; }

    public decimal? TotalModifierAmount { get; set; }

    public decimal? Tax { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Instruction { get; set; }

    public int? ReadyItemQuantity { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public string? Status { get; set; }

    public string? ItemName { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderedItemModifierMapping> OrderedItemModifierMappings { get; set; } = new List<OrderedItemModifierMapping>();
}
