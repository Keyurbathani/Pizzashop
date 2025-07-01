using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class ModifierGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MappingMenuItemWithModifier> MappingMenuItemWithModifiers { get; set; } = new List<MappingMenuItemWithModifier>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<ModifierAndGroup> ModifierAndGroups { get; set; } = new List<ModifierAndGroup>();
}
