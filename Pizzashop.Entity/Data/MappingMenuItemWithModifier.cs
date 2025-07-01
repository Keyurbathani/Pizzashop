using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class MappingMenuItemWithModifier
{
    public int Id { get; set; }

    public int? MenuItemId { get; set; }

    public int? ModifierGroupId { get; set; }

    public int? MaxSelectionRequired { get; set; }

    public int? MinSelectionRequired { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual MenuItem? MenuItem { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ModifierGroup? ModifierGroup { get; set; }
}
